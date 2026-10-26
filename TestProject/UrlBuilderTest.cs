using System.Globalization;
using System.Net;
using System.Text;
using TqkLibrary.Http;

namespace TestProject
{
    [TestClass]
    public class UrlBuilderTest
    {
        [TestMethod]
        public void WithParam_FormatsInvariant_UnderVietnameseCulture()
        {
            CultureInfo old = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("vi-VN");
                string url = new UrlBuilder("api/x")
                    .WithParam("d", 1.5)
                    .WithParam("m", 2.25m)
                    .WithParam("t", new DateTime(2026, 10, 26, 8, 30, 0, DateTimeKind.Utc))
                    .ToString();

                StringAssert.Contains(url, "d=1.5");
                StringAssert.Contains(url, "m=2.25");
                StringAssert.Contains(url, "t=2026-10-26T08%3a30%3a00.0000000Z");
            }
            finally
            {
                CultureInfo.CurrentCulture = old;
            }
        }

        [TestMethod]
        public void WithParam_BoolIsLowercase()
        {
            Assert.AreEqual("api/x?a=true&b=false", new UrlBuilder("api/x").WithParam("a", true).WithParam("b", false).ToString());
        }

        [TestMethod]
        public void WithParamIfNotNull_SkipsNullAndFormatsEnum()
        {
            string url = new UrlBuilder("api/x")
                .WithParamIfNotNull("a", null)
                .WithParamIfNotNull("k", DayOfWeek.Monday)
                .ToString();
            Assert.AreEqual("api/x?k=Monday", url);
        }

        [TestMethod]
        public void ToString_AppendsToExistingQuery()
        {
            Assert.AreEqual("api/x?a=1&b=2", new UrlBuilder("api/x?a=1").WithParam("b", 2).ToString());
        }

        [TestMethod]
        public void ToString_KeepsFragmentLast()
        {
            Assert.AreEqual("api/x?b=2#top", new UrlBuilder("api/x#top").WithParam("b", 2).ToString());
        }

        [TestMethod]
        public void WithParam_ValueTypeCollection_AddsEachItem()
        {
            Assert.AreEqual("api/x?id=1&id=2", new UrlBuilder("api/x").WithParam("id", new List<int> { 1, 2 }).ToString());
        }

        [TestMethod]
        public void WithParam_ByteArray_IsNotExpanded()
        {
            Assert.AreEqual(1, new UrlBuilder("api/x").WithParam("b", new byte[] { 1, 2 }).ToString().Split('&').Length);
        }

        [TestMethod]
        public async Task Request_DoesNotForceHostWithoutPort()
        {
            CaptureHandler handler = new CaptureHandler();
            using CaptureApi api = new CaptureApi(handler);

            await api.Build().WithUrlGet("api/x").ExecuteAsync<string>();

            Assert.IsNotNull(handler.Request);
            Assert.IsTrue(string.IsNullOrEmpty(handler.Request!.Headers.Host) || handler.Request.Headers.Host == "localhost:5001");
            Assert.AreEqual(new Uri("http://localhost:5001/api/x"), handler.Request.RequestUri);
        }

        class CaptureApi : BaseApi
        {
            public CaptureApi(HttpMessageHandler handler)
                : base(new HttpClient(handler, true) { BaseAddress = new Uri("http://localhost:5001/") })
            {
            }

            public new RequestBuilder Build() => base.Build();
        }

        class CaptureHandler : HttpMessageHandler
        {
            public HttpRequestMessage? Request { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Request = request;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("\"ok\"", Encoding.UTF8, "application/json"),
                    RequestMessage = request,
                });
            }
        }
    }
}
