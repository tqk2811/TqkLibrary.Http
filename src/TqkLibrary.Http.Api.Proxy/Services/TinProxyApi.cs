using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Utils;

namespace TqkLibrary.Http.Api.Proxy.Services
{
    /// <summary>
    /// https://tinproxy.com/
    /// </summary>
    public class TinProxyApi : BaseApi
    {
        const string EndPoint = "https://api.tinproxy.com/";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public TinProxyApi(string apiKey) : base(apiKey)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="authenIps"></param>
        /// <returns></returns>
        public Task<TinProxyResponse<TinProxyProxyData>> GetCurrentProxy(CancellationToken cancellationToken = default, params string[] authenIps)
            => GetCurrentProxy(authenIps, cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenIps"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TinProxyResponse<TinProxyProxyData>> GetCurrentProxy(IEnumerable<string>? authenIps = null, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(
                new UrlBuilder(EndPoint, "proxy", "get-current-proxy")
                .WithParam("api_key", ApiKey!)
                .WithParamIfNotNull("authen_ips", authenIps?.Join(",")))
            .ExecuteAsync<TinProxyResponse<TinProxyProxyData>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="authenIps"></param>
        /// <returns></returns>
        public Task<TinProxyResponse<TinProxyProxyData>> GetNewProxy(CancellationToken cancellationToken = default, params string[] authenIps)
           => GetNewProxy(authenIps, cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenIps"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TinProxyResponse<TinProxyProxyData>> GetNewProxy(IEnumerable<string>? authenIps = null, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(
                new UrlBuilder(EndPoint, "proxy", "get-new-proxy")
                .WithParam("api_key", ApiKey!)
                .WithParamIfNotNull("authen_ips", authenIps?.Join(",")))
            .ExecuteAsync<TinProxyResponse<TinProxyProxyData>>(cancellationToken);
    }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TinProxyAuthentication
    {
        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class TinProxyProxyData
    {
        [JsonProperty("http_ipv4")]
        public string HttpIpv4 { get; set; }

        [JsonProperty("http_ipv6")]
        public string HttpIpv6 { get; set; }

        [JsonProperty("socks_ipv4")]
        public string SocksIpv4 { get; set; }

        [JsonProperty("http_ipv6_ipv4")]
        public string HttpIpv6Ipv4 { get; set; }

        [JsonProperty("public_ip")]
        public string PublicIp { get; set; }

        [JsonProperty("public_ip_ipv6")]
        public string PublicIpIpv6 { get; set; }

        [JsonProperty("expired_at")]
        public string ExpiredAt { get; set; }

        [JsonProperty("timeout")]
        public int Timeout { get; set; }

        [JsonProperty("next_request")]
        public int NextRequest { get; set; }

        [JsonProperty("authentication")]
        public TinProxyAuthentication Authentication { get; set; }

        [JsonProperty("ip_allow")]
        public List<string> IpAllows { get; set; }

        [JsonProperty("your_ip")]
        public string YourIp { get; set; }
    }

    public class TinProxyResponse<T>
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}
