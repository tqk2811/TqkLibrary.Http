using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Proxy.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class Proxyv4NetApi : BaseApi
    {
        readonly string EndPoint;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public Proxyv4NetApi(string apiKey) : this(apiKey, "https://api.proxyv4.net/")
        {

        }
        internal Proxyv4NetApi(string apiKey, string endPoint) : base(apiKey)
        {
            if (string.IsNullOrWhiteSpace(endPoint)) throw new ArgumentNullException(nameof(endPoint));
            this.EndPoint = endPoint;
            this._httpClient.BaseAddress = new Uri(endPoint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Proxyv4NetResponse<Proxyv4UserInfo>> UserInfo(string authIp = null, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint + "/api/user-info").WithParam("api_key", ApiKey).WithParamIfNotNull("authIp", authIp))
            .ExecuteAsync<Proxyv4NetResponse<Proxyv4UserInfo>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Proxyv4NetResponse<List<ProxyV4Price>>> ListPrice(string authIp = null, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint + "/api/list-price").WithParam("api_key", ApiKey).WithParamIfNotNull("authIp", authIp))
            .ExecuteAsync<Proxyv4NetResponse<List<ProxyV4Price>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Proxyv4NetResponse<List<ProxyV4Contry>>> ListCountry(string authIp = null, CancellationToken cancellationToken = default)
           => Build()
           .WithUrlGet(new UrlBuilder(EndPoint + "/api/list-country").WithParam("api_key", ApiKey).WithParamIfNotNull("authIp", authIp))
           .ExecuteAsync<Proxyv4NetResponse<List<ProxyV4Contry>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Proxyv4NetResponse<List<ProxyV4Proxy>>> ListProxy(string authIp = null, CancellationToken cancellationToken = default) 
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint + "/api/list-proxy").WithParam("api_key", ApiKey).WithParamIfNotNull("authIp", authIp))
            .ExecuteAsync<Proxyv4NetResponse<List<ProxyV4Proxy>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Proxyv4NetResponse<List<ProxyV4Proxy>>> BuyProxy(int count, ProxyV4Price proxyV4Price, string authIp = null, CancellationToken cancellationToken = default)
            => Build()
                .WithUrlPostJson(
                    new UrlBuilder(EndPoint + "/api/buy-proxy").WithParam("api_key", ApiKey).WithParamIfNotNull("authIp", authIp),
                    new { count, period = proxyV4Price?.Period ?? throw new ArgumentNullException(nameof(proxyV4Price)), country = proxyV4Price?.Country })
                .ExecuteAsync<Proxyv4NetResponse<List<ProxyV4Proxy>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Proxyv4NetResponse<ProxyV4Detail>> ProxyDetail(string proxy_id, string authIp = null, CancellationToken cancellationToken = default)
          => Build()
          .WithUrlGet(new UrlBuilder(EndPoint + "/api/proxy-detail")
              .WithParam("api_key", ApiKey)
              .WithParam("proxy_id", proxy_id)
              .WithParamIfNotNull("authIp", authIp))
          .ExecuteAsync<Proxyv4NetResponse<ProxyV4Detail>>(cancellationToken);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Proxyv4NetResponse> UpdateListProxy(IEnumerable<string> proxy_ids, bool autoRenew = true, string comment = null, string authIp = null, CancellationToken cancellationToken = default)
            => Build()
                .WithUrlPostJson(
                    new UrlBuilder(EndPoint + "/api/update-list-proxy").WithParam("api_key", ApiKey).WithParamIfNotNull("authIp", authIp),
                    new { listProxyId = proxy_ids?.ToList() ?? throw new ArgumentNullException(nameof(proxy_ids)), autoRenew, comment })
                .ExecuteAsync<Proxyv4NetResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Proxyv4NetResponse> RenewListProxy(IEnumerable<string> proxy_ids, int period, string authIp = null, CancellationToken cancellationToken = default)
            => Build()
                .WithUrlPostJson(
                    new UrlBuilder(EndPoint + "/api/renew-list-proxy").WithParam("api_key", ApiKey).WithParamIfNotNull("authIp", authIp),
                    new { listProxyId = proxy_ids?.ToList() ?? throw new ArgumentNullException(nameof(proxy_ids)), period })
                .ExecuteAsync<Proxyv4NetResponse>(cancellationToken);
    }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Proxyv4NetResponse
    {
        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("message")]
        public object Message { get; set; }
    }
    public class Proxyv4NetResponse<T> : Proxyv4NetResponse
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public class Proxyv4UserInfo
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("balance")]
        public int Balance { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }

    public class ProxyV4Price
    {
        [JsonProperty("_id")]
        public string _Id { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("period")]
        public int Period { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("__v")]
        public int V { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class ProxyV4Contry
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class ProxyV4Order
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("orderCode")]
        public string OrderCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }

    public class ProxyV4ProxyInfo
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("proxyId")]
        public ProxyV4ProxyId ProxyId { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("proxyInfo")]
        public string ProxyInfo { get; set; }
    }

    public class ProxyV4ProxyId
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("_id")]
        public string _Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("authIpPort")]
        public int AuthIpPort { get; set; }

        [JsonProperty("socksPort")]
        public int SocksPort { get; set; }

        [JsonProperty("subServerId")]
        public string SubServerId { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("__v")]
        public int V { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class ProxyV4Proxy
    {
        [JsonProperty("renew")]
        public int Renew { get; set; }

        [JsonProperty("autoRenew")]
        public bool AutoRenew { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("_id")]
        public string _Id { get; set; }

        [JsonProperty("user")]
        public ProxyV4User User { get; set; }

        [JsonProperty("proxy")]
        public ProxyV4ProxyInfo Proxy { get; set; }

        [JsonProperty("order")]
        public ProxyV4Order Order { get; set; }

        [JsonProperty("expiresIn")]
        public int ExpiresIn { get; set; }

        [JsonProperty("period")]
        public int Period { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("__v")]
        public int V { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class ProxyV4User
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public class ProxyV4Detail
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("_id")]
        public string _Id { get; set; }

        [JsonProperty("autoRenew")]
        public bool AutoRenew { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("expiresIn")]
        public int ExpiresIn { get; set; }

        [JsonProperty("period")]
        public int Period { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
