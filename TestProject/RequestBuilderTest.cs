using System.Net;
using System.Text;
using TqkLibrary.Http;
using TqkLibrary.Http.HttpClientHandles;

namespace TestProject
{
    [TestClass]
    public class RequestBuilderTest
    {
        [TestMethod]
        public async Task WithBody_DisposeFalse_KeepsContentUsable()
        {
            using StubHandler handler = new StubHandler(_ => Ok());
            using StubApi api = new StubApi(handler);
            using StringContent content = new StringContent("body");

            await api.Build().WithUrlPost("http://localhost/x", content, dispose: false).ExecuteAsync<string>();

            Assert.AreEqual("body", await content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task WithHeader_ContentTypeAndDuplicateKey_DoNotThrow()
        {
            using StubHandler handler = new StubHandler(_ => Ok());
            using StubApi api = new StubApi(handler);

            await api.Build()
                .WithUrlPost("http://localhost/x", new StringContent("body"))
                .WithHeader("X-A", "1")
                .WithHeader("X-A", "2")
                .WithHeader("Content-Language", "vi")
                .ExecuteAsync<string>();

            Assert.AreEqual("2", handler.Requests[0].Headers.GetValues("X-A").Single());
        }

        [TestMethod]
        public async Task WithHeader_ContentType_ReplacesInsteadOfAppending()
        {
            using StubHandler handler = new StubHandler(_ => Ok());
            using StubApi api = new StubApi(handler);

            await api.Build()
                .WithUrlPost("http://localhost/x", new StringContent("body", Encoding.UTF8, "text/plain"))
                .WithHeader("content-type", "application/json")
                .ExecuteAsync<string>();

            Assert.AreEqual("application/json", handler.Requests[0].Content!.Headers.GetValues("Content-Type").Single());
        }

        [TestMethod]
        public async Task Redirect_307TwoHops_ResendsBody()
        {
            List<string?> bodies = new List<string?>();
            int hop = 0;
            using StubHandler inner = new StubHandler(request =>
            {
                bodies.Add(request.Content?.ReadAsStringAsync().Result);
                if (hop++ < 2)
                {
                    HttpResponseMessage redirect = new HttpResponseMessage(HttpStatusCode.TemporaryRedirect);
                    redirect.Headers.Location = new Uri($"http://localhost/hop{hop}");
                    return redirect;
                }
                return Ok();
            });
            using HttpClient client = new HttpClient(new RedirectHandler(inner), false);

            using HttpResponseMessage response = await client.PostAsync("http://localhost/x", new StringContent("body"));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            CollectionAssert.AreEqual(new[] { "body", "body", "body" }, bodies);
        }

        [TestMethod]
        public async Task Redirect_302_KeepsPutAnd303KeepsHead()
        {
            List<HttpMethod> methods = new List<HttpMethod>();
            using StubHandler inner = new StubHandler(request =>
            {
                methods.Add(request.Method);
                if (request.RequestUri!.AbsolutePath == "/start")
                {
                    HttpResponseMessage redirect = new HttpResponseMessage(request.Method == HttpMethod.Head ? HttpStatusCode.SeeOther : HttpStatusCode.Redirect);
                    redirect.Headers.Location = new Uri("http://localhost/end");
                    return redirect;
                }
                return Ok();
            });
            using HttpClient client = new HttpClient(new RedirectHandler(inner), false);

            (await client.PutAsync("http://localhost/start", new StringContent("body"))).Dispose();
            (await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, "http://localhost/start"))).Dispose();

            CollectionAssert.AreEqual(new[] { HttpMethod.Put, HttpMethod.Put, HttpMethod.Head, HttpMethod.Head }, methods);
        }

        [TestMethod]
        public async Task Redirect_HttpsToHttp_IsNotFollowed()
        {
            using StubHandler inner = new StubHandler(_ =>
            {
                HttpResponseMessage redirect = new HttpResponseMessage(HttpStatusCode.Redirect);
                redirect.Headers.Location = new Uri("http://localhost/end");
                return redirect;
            });
            using HttpClient client = new HttpClient(new RedirectHandler(inner), false);

            using HttpResponseMessage response = await client.GetAsync("https://localhost/start");

            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            Assert.AreEqual(1, inner.Requests.Count);
        }

        [TestMethod]
        public async Task Redirect_MaxRedirectCount_FollowsExactlyThatMany()
        {
            using StubHandler inner = new StubHandler(_ =>
            {
                HttpResponseMessage redirect = new HttpResponseMessage(HttpStatusCode.Redirect);
                redirect.Headers.Location = new Uri("http://localhost/loop");
                return redirect;
            });
            using HttpClient client = new HttpClient(new RedirectHandler(inner) { MaxRedirectCount = 3 }, false);

            using HttpResponseMessage response = await client.GetAsync("http://localhost/start");

            Assert.AreEqual(4, inner.Requests.Count);
        }

        [TestMethod]
        public async Task CookieHandler_StoresCookieForFinalUri()
        {
            using StubHandler inner = new StubHandler(request =>
            {
                HttpResponseMessage response = Ok();
                response.Headers.Add("Set-Cookie", "sid=abc; Path=/");
                response.RequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://other.local/final");
                return response;
            });
            CookieHandler cookieHandler = new CookieHandler(inner);
            using HttpClient client = new HttpClient(cookieHandler, false);

            (await client.GetAsync("http://start.local/x")).Dispose();

            Assert.AreEqual("sid=abc", cookieHandler.CookieContainer.GetCookieHeader(new Uri("http://other.local/")));
            Assert.AreEqual(string.Empty, cookieHandler.CookieContainer.GetCookieHeader(new Uri("http://start.local/")));
        }

        static HttpResponseMessage Ok() => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("\"ok\"", Encoding.UTF8, "application/json"),
        };

        class StubApi : BaseApi
        {
            public StubApi(HttpMessageHandler handler) : base(handler, false)
            {
            }

            public new RequestBuilder Build() => base.Build();
        }

        class StubHandler : HttpMessageHandler
        {
            readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;
            public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

            public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
            {
                _respond = respond;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Requests.Add(request);
                HttpResponseMessage response = _respond(request);
                response.RequestMessage ??= request;
                return Task.FromResult(response);
            }
        }
    }
}
