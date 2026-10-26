using System.Net;
using System.Net.Http.Headers;
using TqkLibrary.Http;
using TqkLibrary.Http.Extensions;
using TqkLibrary.Http.Streams;

namespace TestProject
{
    [TestClass]
    public class BearerTokenTest
    {
        class CaptureHandler : HttpMessageHandler
        {
            public List<AuthenticationHeaderValue?> Authorizations { get; } = new List<AuthenticationHeaderValue?>();

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Authorizations.Add(request.Headers.Authorization);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("\"ok\"") });
            }
        }

        class Api : BaseApi
        {
            public Api(HttpMessageHandler handler) : base(handler, false) { }
            public new RequestBuilder Build() => base.Build();
        }

        [TestMethod]
        public async Task SetBearerToken_AttachesToEachRequest_AndClears()
        {
            using CaptureHandler handler = new CaptureHandler();
            using Api api = new Api(handler);

            api.SetBearerToken("abc");
            await api.Build().WithUrlGet("http://localhost/x").ExecuteAsync<string>();
            api.SetBearerToken(null);
            await api.Build().WithUrlGet("http://localhost/x").ExecuteAsync<string>();

            Assert.AreEqual("Bearer abc", handler.Authorizations[0]?.ToString());
            Assert.IsNull(handler.Authorizations[1]);
        }

        [TestMethod]
        public async Task SetBearerToken_DoesNotOverrideExplicitAuthorizationHeader()
        {
            using CaptureHandler handler = new CaptureHandler();
            using Api api = new Api(handler);

            api.SetBearerToken("abc");
            await api.Build().WithUrlGet("http://localhost/x").WithHeader("Authorization", "Basic xyz").ExecuteAsync<string>();

            Assert.AreEqual("Basic xyz", handler.Authorizations[0]?.ToString());
        }
    }
}
