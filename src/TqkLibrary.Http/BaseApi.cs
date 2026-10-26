using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TqkLibrary.Http.Json;
using System;
using System.Net;
using System.Net.Http;
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
        public readonly string? ApiKey;

        /// <summary>
        ///
        /// </summary>
        internal protected readonly HttpClient _httpClient;

        /// <summary>
        ///
        /// </summary>
        internal protected JsonSerializerSettings? DefaultJsonSerializerSettings { get; set; } = CreateDefaultJsonSerializerSettings();

        static JsonSerializerSettings CreateDefaultJsonSerializerSettings() => new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCaseIgnoreEmptyStringContractResolver(),
#if DEBUG
            MissingMemberHandling = MissingMemberHandling.Error,
#endif
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="httpMessageHandler"></param>
        /// <param name="disposeHandler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected BaseApi(string apiKey, HttpMessageHandler httpMessageHandler, bool disposeHandler = false)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            this._httpClient = new HttpClient(httpMessageHandler ?? throw new ArgumentNullException(nameof(httpMessageHandler)), disposeHandler);
            this.ApiKey = apiKey;
        }
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

        protected BaseApi(string apiKey, HttpClient httpClient)
        {
            this.ApiKey = apiKey;
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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

        protected internal virtual Task OnBeforeRequestAsync(HttpRequestMessage httpRequestMessage)
        {
            return Task.CompletedTask;
        }
        protected internal virtual Task OnAfterRequestAsync(HttpResponseMessage httpResponseMessage)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string? ToString()
        {
            if (string.IsNullOrWhiteSpace(ApiKey))
            {
                return base.ToString();
            }
            else
            {
                return $"{this.GetType().Name}({ApiKey})";
            }
        }
    }
}