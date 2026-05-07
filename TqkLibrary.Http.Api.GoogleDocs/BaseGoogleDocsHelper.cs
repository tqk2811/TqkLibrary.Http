using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.HttpClientHandles;
using System.Net.Http.Headers;

namespace TqkLibrary.Http.Api.GoogleDocs
{
    public abstract class BaseGoogleDocsHelper : BaseApi
    {
        public BaseGoogleDocsHelper() : this(
#if NET5_0_OR_GREATER
            new SocketsHttpHandler() { AllowAutoRedirect = false }.DisableFindIpV6()
#else
            new HttpClientHandler() { AllowAutoRedirect = false }
#endif
            )
        {

        }
        public BaseGoogleDocsHelper(HttpMessageHandler httpMessageHandler) : this(new CookieHandler(httpMessageHandler))
        {
            this._httpClient.DefaultRequestHeaders.Referrer = new Uri("https://docs.google.com");
            this._httpClient.DefaultRequestHeaders.Add("Origin", "https://docs.google.com/");
        }

        readonly CookieHandler _cookieHandler;
        private BaseGoogleDocsHelper(CookieHandler cookieHandler) : base(".", cookieHandler)
        {
            this._cookieHandler = cookieHandler;
        }


        protected async Task<Stream> HandlerRedirect(UrlBuilder urlBuilder, CancellationToken cancellationToken = default)
        {
            using HttpResponseMessage res = await Build()
                .WithUrlGet(urlBuilder)
                .ExecuteAsync(cancellationToken);
            if (res.Headers.Location is null)
            {
                res.EnsureSuccessStatusCode();
                throw new InvalidOperationException($"Location was null and StatusCode: {res.StatusCode}");
            }
            //string body = await res.EnsureSuccessStatusCode().Content.ReadAsStringAsync();

            HttpResponseMessage res2 = await Build()
                .WithUrlGet(res.Headers.Location!)
                .ExecuteAsync(cancellationToken);
#if DEBUG
            if (!res2.IsSuccessStatusCode)
            {
                string body2 = await res2.Content.ReadAsStringAsync();
            }
#endif
            try
            {
                var stream = await res2.EnsureSuccessStatusCode().Content.ReadAsStreamAsync();
                return new HttpResponseStreamWrapper(res2, stream);
            }
            catch
            {
                res2.Dispose();
                throw;
            }
        }
    }
}
