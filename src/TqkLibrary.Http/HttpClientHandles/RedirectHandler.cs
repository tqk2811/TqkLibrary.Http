using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.HttpClientHandles
{
    /// <summary>
    /// Follows 3xx redirects by issuing new requests through base.SendAsync.
    /// Place this OUTSIDE of <see cref="CookieHandler"/> so each redirect hop re-applies cookies
    /// from and captures Set-Cookie into the cookie container.
    /// The inner handler must have AllowAutoRedirect = false, otherwise it follows redirects itself
    /// and this handler never sees a 3xx response.
    /// </summary>
    public class RedirectHandler : DelegatingHandler
    {
        /// <summary>
        ///
        /// </summary>
        public RedirectHandler(HttpMessageHandler innerHandler) : base(innerHandler ?? throw new ArgumentNullException(nameof(innerHandler)))
        {

        }
        /// <summary>
        ///
        /// </summary>
        public int MaxRedirectCount { get; set; } = 50;
        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var currentRequest = request;
            var response = await base.SendAsync(currentRequest, cancellationToken).ConfigureAwait(false);
            int redirectCount = 0;
            while (true)
            {
                if (response.Headers.Location is null)
                    return response;

                // Same rules as browsers / SocketsHttpHandler:
                //  - 301/302: only POST becomes GET (body dropped), other methods are kept.
                //  - 303: everything except HEAD becomes GET.
                //  - 307/308: keep method and body.
                bool keepMethod;
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Moved: // 301
                    case HttpStatusCode.Redirect: // 302
                        keepMethod = currentRequest.Method != HttpMethod.Post;
                        break;

                    case HttpStatusCode.RedirectMethod: // 303
                        keepMethod = currentRequest.Method == HttpMethod.Head;
                        break;

                    case HttpStatusCode.RedirectKeepVerb: // 307
#if NET5_0_OR_GREATER
                    case HttpStatusCode.PermanentRedirect: // 308
#else
                    case (HttpStatusCode)308:
#endif
                        keepMethod = true;
                        break;

                    default:
                        return response;
                }

                Uri nextUri = ResolveLocation(currentRequest.RequestUri, response.Headers.Location);
                // Do not downgrade https -> http: headers would travel over an unencrypted channel
                if (currentRequest.RequestUri?.Scheme == Uri.UriSchemeHttps && nextUri.Scheme != Uri.UriSchemeHttps)
                    return response;

                redirectCount++;
                if (redirectCount > MaxRedirectCount)
                    return response;

                HttpRequestMessage nextRequest = new HttpRequestMessage(keepMethod ? currentRequest.Method : HttpMethod.Get, nextUri);
                CopyHeaders(currentRequest, nextRequest);
                if (keepMethod && currentRequest.Content != null)
                {
                    // Move the body to the next hop so disposing the previous hop does not dispose it
                    nextRequest.Content = currentRequest.Content;
                    if (!ReferenceEquals(currentRequest, request))
                        currentRequest.Content = null;
                }

                response.Dispose();
                if (!ReferenceEquals(currentRequest, request))
                    currentRequest.Dispose();
                currentRequest = nextRequest;
                response = await base.SendAsync(currentRequest, cancellationToken).ConfigureAwait(false);
            }
        }

        static Uri ResolveLocation(Uri? baseUri, Uri location)
        {
            if (location.IsAbsoluteUri) return location;
            if (baseUri is null) throw new InvalidOperationException("cannot resolve relative redirect without base uri");
            return new Uri(baseUri, location);
        }

        // Copy request headers to the redirected request, except:
        //  - Host: recomputed from the new URI by HttpClient.
        //  - Cookie: the CookieHandler in the chain will repopulate this from the (now-updated)
        //    cookie container for the new request URI.
        //  - Authorization: drop on cross-origin redirect to avoid leaking credentials.
        static void CopyHeaders(HttpRequestMessage src, HttpRequestMessage dst)
        {
            bool sameOrigin = src.RequestUri != null
                && dst.RequestUri != null
                && string.Equals(src.RequestUri.Host, dst.RequestUri.Host, StringComparison.OrdinalIgnoreCase)
                && src.RequestUri.Scheme.Equals(dst.RequestUri.Scheme, StringComparison.OrdinalIgnoreCase)
                && src.RequestUri.Port == dst.RequestUri.Port;

            foreach (var pair in src.Headers)
            {
                if (string.Equals(pair.Key, "Host", StringComparison.OrdinalIgnoreCase)) continue;
                if (string.Equals(pair.Key, "Cookie", StringComparison.OrdinalIgnoreCase)) continue;
                if (!sameOrigin && string.Equals(pair.Key, "Authorization", StringComparison.OrdinalIgnoreCase)) continue;
                dst.Headers.TryAddWithoutValidation(pair.Key, pair.Value);
            }
        }
    }
}
