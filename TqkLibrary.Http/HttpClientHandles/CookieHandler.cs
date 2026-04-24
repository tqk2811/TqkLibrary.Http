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
        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _semaphore.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            string cookie = CookieContainer.GetCookieHeader(request.RequestUri);
            if (!string.IsNullOrEmpty(cookie)) request.Headers.Add("Cookie", cookie);

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.Headers.TryGetValues("Set-Cookie", out var newCookies))
            {
                await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    foreach (var cookieHeader in newCookies)
                    {
                        try
                        {
                            // Dùng CookieContainer.SetCookies (native RFC 6265 parser của .NET) thay cho
                            // SetCookieHeaderValue parse thủ công — tránh edge case không tương thích với
                            // behavior mặc định của SocketsHttpHandler khi UseCookies=true.
                            CookieContainer.SetCookies(request.RequestUri, cookieHeader);
                        }
                        catch (CookieException)
                        {
                            // malformed cookie: ignore (matches browser behaviour)
                        }
                    }
                }
                finally
                {
                    _semaphore.Release();
                }

                ThreadPool.QueueUserWorkItem(_ => CookieChanged?.Invoke(this, CookieContainer));
            }

            return response;
        }
    }
}
