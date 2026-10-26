using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.HttpClientHandles
{
    //https://gist.github.com/damianh/038195c1ab0c5013ad3883d7e3c59d99
    /// <summary>
    /// 
    /// </summary>
    public class CookieHandler : DelegatingHandler
    {
        /// <summary>
        ///
        /// </summary>
        public CookieContainer CookieContainer { get; }

        /// <summary>
        ///
        /// </summary>
        public event EventHandler<CookieContainer>? CookieChanged;

        /// <summary>
        /// 
        /// </summary>
        public CookieHandler(HttpMessageHandler innerHandler) : base(innerHandler ?? throw new ArgumentNullException(nameof(innerHandler)))
        {
            this.CookieContainer = new CookieContainer();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public CookieHandler(CookieContainer cookieContainer, HttpMessageHandler innerHandler) : base(innerHandler ?? throw new ArgumentNullException(nameof(innerHandler)))
        {
            this.CookieContainer = cookieContainer ?? throw new ArgumentNullException(nameof(cookieContainer));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            string cookie = CookieContainer.GetCookieHeader(request.RequestUri);
            if (!string.IsNullOrEmpty(cookie)) request.Headers.Add("Cookie", cookie);

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.Headers.TryGetValues("Set-Cookie", out var newCookies))
            {
                // Inner handler có thể đã tự theo redirect: cookie thuộc về URI cuối cùng, không phải URI gốc
                Uri cookieUri = response.RequestMessage?.RequestUri ?? request.RequestUri!;
                // CookieContainer tự thread-safe nên không cần khoá; khoá cũ còn làm rò response khi bị huỷ lúc chờ
                foreach (var cookieHeader in newCookies)
                {
                    try
                    {
                        // Dùng CookieContainer.SetCookies (native RFC 6265 parser của .NET) thay cho
                        // SetCookieHeaderValue parse thủ công — tránh edge case không tương thích với
                        // behavior mặc định của SocketsHttpHandler khi UseCookies=true.
                        CookieContainer.SetCookies(cookieUri, cookieHeader);
                    }
                    catch (CookieException)
                    {
                        // malformed cookie: ignore (matches browser behaviour)
                    }
                }

                ThreadPool.QueueUserWorkItem(_ => CookieChanged?.Invoke(this, CookieContainer));
            }

            return response;
        }
    }
}
