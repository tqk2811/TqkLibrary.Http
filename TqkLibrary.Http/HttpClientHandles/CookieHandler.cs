using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
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
                    foreach (var item in SetCookieHeaderValue.ParseList(newCookies.ToList()))
                    {
                        var newCookie = new Cookie(
                            item.Name.Value,
                            item.Value.HasValue ? item.Value.Value : string.Empty)
                        {
                            Secure = item.Secure,
                            HttpOnly = item.HttpOnly,
                        };
                        if (item.Path.HasValue)
                            newCookie.Path = item.Path.Value;
                        if (item.Domain.HasValue && DomainMatches(request.RequestUri.Host, item.Domain.Value))
                            newCookie.Domain = item.Domain.Value;
                        if (item.MaxAge.HasValue)
                            newCookie.Expires = DateTime.UtcNow.Add(item.MaxAge.Value);
                        else if (item.Expires.HasValue)
                            newCookie.Expires = item.Expires.Value.UtcDateTime;

                        try
                        {
                            CookieContainer.Add(request.RequestUri, newCookie);
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

        static bool DomainMatches(string host, string domain)
        {
            if (string.IsNullOrEmpty(domain)) return false;
            string d = domain[0] == '.' ? domain.Substring(1) : domain;
            if (d.Length == 0) return false;
            return host.Equals(d, StringComparison.OrdinalIgnoreCase)
                || host.EndsWith("." + d, StringComparison.OrdinalIgnoreCase);
        }
    }
}
