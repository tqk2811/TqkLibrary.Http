using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

        [Obsolete("use _httpClient instead")]
        internal protected readonly HttpClient httpClient;
        /// <summary>
        /// 
        /// </summary>
        internal protected readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        internal protected JsonSerializerSettings? DefaultJsonSerializerSettings { get; set; } = NetSingleton.JsonSerializerSettings;
        /// <summary>
        /// Dùng handler chung của cả tiến trình (<see cref="NetSingleton.HttpClientHandler"/>).
        /// </summary>
        /// <remarks>
        /// Handler đó là MỘT instance cho cả tiến trình, nên mọi đối tượng dựng qua constructor này
        /// chia nhau cùng một handler. <see cref="HttpClient"/> thì riêng từng đối tượng, nên
        /// <c>DefaultRequestHeaders</c> (ví dụ bearer token) không rò sang đối tượng khác — nhưng
        /// thứ gì nằm TRONG handler thì có. Cookie đã được tắt sẵn vì lý do đó. Cần cookie riêng
        /// từng phiên thì dùng constructor nhận <see cref="HttpMessageHandler"/> và bọc
        /// <see cref="HttpClientHandles.CookieHandler"/>.
        /// </remarks>
        protected BaseApi()
        {
            this._httpClient = new HttpClient(NetSingleton.HttpClientHandler, false);
            this.httpClient = this._httpClient;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected BaseApi(string apiKey) : this()
        {
            if (string.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            this.ApiKey = apiKey;
        }
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
            this.httpClient = this._httpClient;
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
            this.httpClient = this._httpClient;
        }

        protected BaseApi(string apiKey, HttpClient httpClient)
        {
            this.ApiKey = apiKey;
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.httpClient = this._httpClient;
        }
        protected BaseApi(HttpClient httpClient)
        {
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.httpClient = this._httpClient;
        }

        /// <summary>
        /// 
        /// </summary>
        ~BaseApi()
        {
            Dispose(false);
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