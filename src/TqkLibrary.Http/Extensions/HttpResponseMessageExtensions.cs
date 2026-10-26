using System;
using System.Net.Http;

namespace TqkLibrary.Http.Extensions
{
    /// <summary>
    ///
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// How long the server asks to wait before retrying, read from the <c>Retry-After</c> header
        /// (seconds or HTTP date). Null when the header is missing or the date is already past.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TimeSpan? GetRetryAfter(this HttpResponseMessage response)
        {
            if (response is null) throw new ArgumentNullException(nameof(response));

            var header = response.Headers.RetryAfter;
            if (header?.Delta != null)
                return header.Delta;
            if (header?.Date != null)
            {
                var wait = header.Date.Value - DateTimeOffset.UtcNow;
                if (wait > TimeSpan.Zero)
                    return wait;
            }

            return null;
        }
    }
}
