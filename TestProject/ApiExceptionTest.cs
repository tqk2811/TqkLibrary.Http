using System.Net;
using System.Text;
using TqkLibrary.Http;

namespace TestProject
{
    [TestClass]
    public class ApiExceptionTest
    {
        const string _url = "https://example.com/v1/resource";

        [TestMethod]
        public void Message_IsBuiltFromContext_WhenNotGiven()
        {
            using HttpResponseMessage response = CreateResponse(HttpStatusCode.TooManyRequests, "{\"error\":\"wait 34s\"}", "application/json");
            ApiException exception = new ApiException(response, "{\"error\":\"wait 34s\"}");

            StringAssert.Contains(exception.Message, "GET");
            StringAssert.Contains(exception.Message, _url);
            StringAssert.Contains(exception.Message, "429");
            StringAssert.Contains(exception.Message, "wait 34s");
        }

        [TestMethod]
        public void Message_KeepsExplicitMessage()
        {
            ApiException exception = new ApiException("something specific")
            {
                StatusCode = HttpStatusCode.BadGateway,
            };

            Assert.AreEqual("something specific", exception.Message);
        }

        [TestMethod]
        public void Message_IsSingleLine_ForPrettyPrintedBody()
        {
            string body = "{\r\n  \"error\": \"nope\"\r\n}";
            using HttpResponseMessage response = CreateResponse(HttpStatusCode.BadRequest, body, "application/json");
            ApiException exception = new ApiException(response, body);

            Assert.IsFalse(exception.Message.Contains("\n"), "Message must stay on one line");
            StringAssert.Contains(exception.Message, "\"error\": \"nope\"");
        }

        [TestMethod]
        public void LongHtmlBody_PrintsOnlyTypeAndLength()
        {
            string body = "<html><body>" + new string('x', 50000) + "</body></html>";
            using HttpResponseMessage response = CreateResponse(HttpStatusCode.BadGateway, body, "text/html");
            ApiException exception = new ApiException(response, body);

            StringAssert.Contains(exception.Message, "text/html");
            StringAssert.Contains(exception.Message, $"{body.Length} chars");
            Assert.IsFalse(exception.Message.Contains("<html>"), "The body itself must not be printed");
            Assert.IsFalse(exception.ToString().Contains("<html>"), "The body itself must not be printed");
            StringAssert.Contains(exception.ToString(), "body omitted");
        }

        [TestMethod]
        public void ShortHtmlBody_IsPrinted()
        {
            const string body = "<h1>502 Bad Gateway</h1>";
            using HttpResponseMessage response = CreateResponse(HttpStatusCode.BadGateway, body, "text/html");
            ApiException exception = new ApiException(response, body);

            StringAssert.Contains(exception.Message, body);
        }

        [TestMethod]
        public void BinaryBody_PrintsBytes_NotContent()
        {
            byte[] bytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x01, 0x02, 0x03 };
            using HttpResponseMessage response = CreateBinaryResponse(HttpStatusCode.OK, bytes, "application/octet-stream");
            string mangled = Encoding.UTF8.GetString(bytes);
            ApiException exception = new ApiException(response, mangled);

            Assert.IsFalse(exception.IsBodyText);
            StringAssert.Contains(exception.Message, "application/octet-stream");
            StringAssert.Contains(exception.Message, $"{bytes.Length} bytes");
            StringAssert.Contains(exception.ToString(), "body omitted: not text");
        }

        [TestMethod]
        public void BinaryBody_IsDetected_EvenWhenContentTypeSaysText()
        {
            byte[] bytes = new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x00, 0x01 };
            using HttpResponseMessage response = CreateBinaryResponse(HttpStatusCode.OK, bytes, "text/plain");
            ApiException exception = new ApiException(response, Encoding.UTF8.GetString(bytes));

            Assert.IsFalse(exception.IsBodyText, "Control characters must win over the content type");
            StringAssert.Contains(exception.Message, "bytes");
        }

        [TestMethod]
        public void BodyObject_IsReadable_WithoutTypeArgument()
        {
            using HttpResponseMessage response = CreateResponse(HttpStatusCode.BadRequest, "{\"error\":\"nope\"}", "application/json");
            ApiException exception = new ApiException<ErrorBody>(response, "{\"error\":\"nope\"}", new ErrorBody { Error = "nope" });

            Assert.IsInstanceOfType(exception.BodyObject, typeof(ErrorBody));
            Assert.AreEqual("nope", ((ErrorBody)exception.BodyObject!).Error);
        }

        [TestMethod]
        public void Data_CarriesContext_ForStructuredLogSinks()
        {
            using HttpResponseMessage response = CreateResponse(HttpStatusCode.NotFound, "{}", "application/json");
            ApiException exception = new ApiException(response, "{}");

            Assert.AreEqual(404, exception.Data[nameof(ApiException.StatusCode)]);
            Assert.AreEqual(_url, exception.Data[nameof(ApiException.RequestUri)]);
            Assert.AreEqual("GET", exception.Data[nameof(ApiException.RequestMethod)]);
        }

        [TestMethod]
        public void Data_OmitsHugeBody()
        {
            string body = new string('x', 50000);
            using HttpResponseMessage response = CreateResponse(HttpStatusCode.BadGateway, body, "text/html");
            ApiException exception = new ApiException(response, body);

            Assert.IsNull(exception.Data[nameof(ApiException.RawBody)]);
            Assert.AreEqual(body.Length, exception.Data[nameof(ApiException.BodyLength)]);
        }

        [TestMethod]
        public async Task Execute_NonJsonErrorBody_ThrowsApiException_NotJsonReaderException()
        {
            const string body = "<html><body>gateway down</body></html>";
            using FakeApi api = new FakeApi(HttpStatusCode.BadGateway, body, "text/html");

            ApiException<ErrorBody> exception = await Assert.ThrowsExceptionAsync<ApiException<ErrorBody>>(
                () => api.Build().WithUrlGet(_url).ExecuteAsync<ErrorBody, ErrorBody>());

            Assert.AreEqual(ApiErrorKind.HttpStatus, exception.Kind);
            Assert.AreEqual(HttpStatusCode.BadGateway, exception.StatusCode);
            Assert.AreEqual(body, exception.RawBody);
            Assert.IsNull(exception.Body, "An HTML body cannot become ErrorBody");
            Assert.IsNotNull(exception.InnerException, "The deserialization error must be kept");
            Assert.AreEqual(_url, exception.RequestUri?.ToString());
            Assert.AreEqual("GET", exception.RequestMethod?.Method);
        }

        [TestMethod]
        public async Task Execute_ErrorBody_IsDeserialized()
        {
            const string body = "{\"error\":\"wait 34s\"}";
            using FakeApi api = new FakeApi(HttpStatusCode.TooManyRequests, body, "application/json");

            ApiException<ErrorBody> exception = await Assert.ThrowsExceptionAsync<ApiException<ErrorBody>>(
                () => api.Build().WithUrlGet(_url).ExecuteAsync<ErrorBody, ErrorBody>());

            Assert.AreEqual("wait 34s", exception.Body?.Error);
            StringAssert.Contains(exception.Message, "429");
            StringAssert.Contains(exception.Message, "wait 34s");
        }

        [TestMethod]
        public async Task Execute_SuccessStatusButBadJson_ReportsDeserializeFailed()
        {
            const string body = "this is not json {";
            using FakeApi api = new FakeApi(HttpStatusCode.OK, body, "text/plain");

            ApiException<ErrorBody> exception = await Assert.ThrowsExceptionAsync<ApiException<ErrorBody>>(
                () => api.Build().WithUrlGet(_url).ExecuteAsync<ErrorBody, ErrorBody>());

            Assert.AreEqual(ApiErrorKind.DeserializeFailed, exception.Kind);
            Assert.AreEqual(HttpStatusCode.OK, exception.StatusCode);
            StringAssert.Contains(exception.Message, nameof(ApiErrorKind.DeserializeFailed));
            Assert.IsNotNull(exception.InnerException);
        }

        static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string content, string contentType)
            => new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, Encoding.UTF8, contentType),
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, _url),
            };

        static HttpResponseMessage CreateBinaryResponse(HttpStatusCode statusCode, byte[] content, string contentType)
        {
            ByteArrayContent byteArrayContent = new ByteArrayContent(content);
            byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            return new HttpResponseMessage(statusCode)
            {
                Content = byteArrayContent,
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, _url),
            };
        }

        public class ErrorBody
        {
            public string? Error { get; set; }
        }

        class FakeApi : BaseApi
        {
            public FakeApi(HttpStatusCode statusCode, string content, string contentType)
                : base(new FakeHandler(statusCode, content, contentType), true)
            {
            }

            public new RequestBuilder Build() => base.Build();
        }

        class FakeHandler : HttpMessageHandler
        {
            readonly HttpStatusCode _statusCode;
            readonly string _content;
            readonly string _contentType;

            public FakeHandler(HttpStatusCode statusCode, string content, string contentType)
            {
                _statusCode = statusCode;
                _content = content;
                _contentType = contentType;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(new HttpResponseMessage(_statusCode)
                {
                    Content = new StringContent(_content, Encoding.UTF8, _contentType),
                    RequestMessage = request,
                });
        }
    }
}
