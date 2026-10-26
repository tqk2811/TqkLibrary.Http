using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
namespace TqkLibrary.Http
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseApi : IDisposable
    {
        internal protected ILogger? _logger;

        /// <summary>
        ///
        /// </summary>
        internal protected readonly HttpClient _httpClient;

        /// <summary>
        /// Settings used to serialize request bodies and deserialize responses. Null uses <see cref="JsonConvert.DefaultSettings"/>.
        /// </summary>
        internal protected JsonSerializerSettings? DefaultJsonSerializerSettings { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpMessageHandler"></param>
        /// <param name="disposeHandler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected BaseApi(HttpMessageHandler httpMessageHandler, bool disposeHandler = false)
        {
            this._httpClient = new HttpClient(httpMessageHandler ?? throw new ArgumentNullException(nameof(httpMessageHandler)), disposeHandler);
        }

        protected BaseApi(HttpClient httpClient)
        {
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _httpClient.Dispose();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual RequestBuilder Build() => new RequestBuilder(this);

        /// <summary>
        /// Read on every send while a token refresh may write it concurrently, hence <c>volatile</c>.
        /// </summary>
        volatile AuthenticationHeaderValue? _bearer;
        internal AuthenticationHeaderValue? Bearer => _bearer;

        /// <summary>
        /// Sets (or clears with null/empty) the bearer token attached to every following request of this client,
        /// unless the request already has an <c>Authorization</c> header.
        /// <para>
        /// Does NOT write <c>DefaultRequestHeaders</c>: <see cref="System.Net.Http.Headers.HttpHeaders"/> is not
        /// thread-safe and <see cref="HttpClient"/> reads the default headers while sending, so refreshing the
        /// token during parallel calls would race with them. The token is attached per request instead.
        /// </para>
        /// </summary>
        /// <param name="token"></param>
        public virtual void SetBearerToken(string? token)
        {
            _bearer = string.IsNullOrWhiteSpace(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
        }

        protected internal virtual Task OnBeforeRequestAsync(HttpRequestMessage httpRequestMessage)
        {
            return Task.CompletedTask;
        }
        protected internal virtual Task OnAfterRequestAsync(HttpResponseMessage httpResponseMessage)
        {
            return Task.CompletedTask;
        }
    }
}