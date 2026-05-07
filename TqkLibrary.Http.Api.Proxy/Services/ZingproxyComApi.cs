using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace TqkLibrary.Http.Api.Proxy.Services
{
    public class ZingproxyComApi : BaseApi
    {
        const string EndPoint = "https://api.zingproxy.com";
        public ZingproxyComApi(string accessToken) : this(accessToken, NetSingleton.HttpClientHandler, false)
        {

        }
        public ZingproxyComApi(string accessToken, HttpMessageHandler httpMessageHandler, bool disposeHandler = false) : base(accessToken, httpMessageHandler, disposeHandler)
        {
            this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            this.DanCuVietNam = new(this);
        }

        public DanCuVietNamApi DanCuVietNam { get; }

        public class DanCuVietNamApi
        {
            const string EndPoint = "https://api.zingproxy.com/proxy/dan-cu-viet-nam";
            readonly ZingproxyComApi _zingproxyComApi;
            internal DanCuVietNamApi(ZingproxyComApi zingproxyComApi)
            {
                this._zingproxyComApi = zingproxyComApi ?? throw new ArgumentNullException(nameof(zingproxyComApi));
            }


            public Task<ProxiesResponse<ProxyFullInfo>> ListAsync(RunningType runningType = RunningType.Running, CancellationToken cancellationToken = default)
                => _zingproxyComApi.Build()
                .WithUrlGet(new UrlBuilder(EndPoint, runningType.ToString().ToLower()))
                .ExecuteAsync<ProxiesResponse<ProxyFullInfo>>(cancellationToken);

            /// <summary>
            /// get and change ip
            /// </summary>
            public Task<ProxyResponse<ProxyFullInfo>> GetIpAsync(ProxyFullInfo proxyFullInfo, string? location = "Random", CancellationToken cancellationToken = default)
                => GetIpAsync(proxyFullInfo?.UId!, location, cancellationToken);
            /// <summary>
            /// get and change ip
            /// </summary>
            public Task<ProxyResponse<ProxyFullInfo>> GetIpAsync(string uid, string? location = "Random", CancellationToken cancellationToken = default)
            {
                if (string.IsNullOrWhiteSpace(uid)) throw new ArgumentNullException(nameof(uid));
                return _zingproxyComApi.Build()
                    .WithUrlGet(new UrlBuilder(EndPoint, "get-ip")
                        .WithParam("uId", uid)
                        .WithParamIfNotNull("location", location))
                    .ExecuteAsync<ProxyResponse<ProxyFullInfo>, BaseResponse>(cancellationToken);
            }
        }







        public Task<ProxyResponse<ProxyInfo>> GetProxy(ProxyFullInfo proxyFullInfo, CancellationToken cancellationToken = default)
            => GetProxy(proxyFullInfo.Key!, cancellationToken);
        public Task<ProxyResponse<ProxyInfo>> GetProxy(string key, CancellationToken cancellationToken = default)
            => Build()
                .WithUrlGet(new UrlBuilder(EndPoint, "get-proxy").WithParam("key", key))
                .ExecuteAsync<ProxyResponse<ProxyInfo>>(cancellationToken);














        public class ProxyInfo
        {
            [JsonProperty("uId")]
            public string? UId { get; set; }

            [JsonProperty("resourceId")]
            public string? ResourceId { get; set; }

            [JsonProperty("httpProxy")]
            public string? HttpProxy { get; set; }

            [JsonProperty("socks5Proxy")]
            public string? Socks5Proxy { get; set; }

            [JsonProperty("createdAt")]
            public DateTime? CreatedAt { get; set; }

            [JsonProperty("dateEnd")]
            public DateTime? DateEnd { get; set; }

            [JsonProperty("autoRenew")]
            public bool? AutoRenew { get; set; }
        }

        public class ProxyFullInfo
        {
            [JsonIgnore]
            public DateTime ObjectCreateTime { get; } = DateTime.Now;

            [JsonProperty("uId")]
            public string? UId { get; set; }

            [JsonProperty("resourceId")]
            public string? ResourceId { get; set; }

            [JsonProperty("key")]
            public string? Key { get; set; }

            [JsonProperty("state")]
            public string? State { get; set; }

            [JsonProperty("ip")]
            public string? Ip { get; set; }

            [JsonProperty("hostIp")]
            public string? HostIp { get; set; }

            [JsonProperty("portHttp")]
            public int? PortHttp { get; set; }

            [JsonProperty("portSocks5")]
            public int? PortSocks5 { get; set; }

            [JsonProperty("username")]
            public string? Username { get; set; }

            [JsonProperty("password")]
            public string? Password { get; set; }

            [JsonProperty("countryCode")]
            public string? CountryCode { get; set; }

            [JsonProperty("whiteListIp")]
            public List<string>? WhiteListIp { get; set; }

            [JsonProperty("ispNetworks")]
            public List<string>? IspNetworks { get; set; }

            [JsonProperty("linkChangeIp")]
            public string? LinkChangeIp { get; set; }

            [JsonProperty("createdAt")]
            public DateTime? CreatedAt { get; set; }

            [JsonProperty("dateEnd")]
            public DateTime? DateEnd { get; set; }

            [JsonProperty("prices")]
            public Dictionary<string, int>? Prices { get; set; }

            [JsonProperty("timeChangeAllowInSeconds")]
            public int? TimeChangeAllowInSeconds { get; set; }

            [JsonProperty("setTimeAutoChangeIP")]
            public int? SetTimeAutoChangeIP { get; set; }

            [JsonProperty("autoRenew")]
            public bool? AutoRenew { get; set; }
        }

        public class BaseResponse
        {
            [JsonProperty("status")]
            public string? Status { get; set; }

            [JsonProperty("statusCode")]
            public int? StatusCode { get; set; }

            [JsonProperty("error")]
            public string? Error { get; set; }

            [JsonProperty("message")]
            public string? Message { get; set; }

            [JsonProperty("warningSpam")]
            public string? WarningSpam { get; set; }

            [JsonProperty("uId")]
            public string? UId { get; set; }
        }

        public class ProxyResponse<T> : BaseResponse
        {
            [JsonProperty("proxy")]
            public T? Proxy { get; set; }
        }
        public class ProxiesResponse<T> : BaseResponse
        {
            [JsonProperty("proxies")]
            public List<T>? Proxies { get; set; }
        }

        public enum RunningType
        {
            Running,
            Expiring,
            Cancelled,
            All
        }
    }
}
