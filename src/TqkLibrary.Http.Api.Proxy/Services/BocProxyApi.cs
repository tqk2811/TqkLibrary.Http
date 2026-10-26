using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace TqkLibrary.Http.Api.Proxy.Services
{
    /// <summary>
    /// https://github.com/bocproxy/dcom-proxy/wiki/API-Documentation
    /// </summary>
    public class BocProxyApi : BaseApi
    {
        readonly Uri _domain;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BocProxyApi(Uri endpoint)
        {
            this._domain = endpoint?.GetDomain() ?? throw new ArgumentNullException(nameof(endpoint));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<List<ObcProxy>> ProxyList(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(_domain, "proxy_list"))
            .ExecuteAsync<List<ObcProxy>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obcProxy"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ObcStatus> Reset(ObcProxy obcProxy, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(_domain, "reset").WithParam("proxy", obcProxy.ProxyPort))
            .ExecuteAsync<ObcStatus>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ObcStatus> ResetAll(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(_domain, "reset_all"))
            .ExecuteAsync<ObcStatus>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obcProxy"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ObcStatus> Status(ObcProxy obcProxy, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(_domain, "status").WithParam("proxy", obcProxy.ProxyPort))
            .ExecuteAsync<ObcStatus>(cancellationToken);


        //public Task PublicIp(ObcProxy obcProxy,CancellationToken cancellationToken = default)
        //    => Build()
        //    .WithUrlGet(new UriBuilder(_endPoint.GetDomain(), "public_ip").WithParam("proxy", obcProxy.ProxyPort))
        //    .WithCancellationToken(cancellationToken)
        //    .ExecuteAsync<ObcStatus>();








        /// <summary>
        /// 
        /// </summary>
        public class ObcStatus
        {
            /// <summary>
            /// 
            /// </summary>
            [JsonProperty("status")]
            public bool Status { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonProperty("msg")]
            public string? Message { get; set; }
        }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public class ObcProxy
        {
            [JsonProperty("imei")]
            public string? Imei { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("provider")]
            public string? Provider { get; set; }

            [JsonProperty("ip")]
            public string? Ip { get; set; }

            [JsonProperty("ipv6")]
            public object? IpV6 { get; set; }

            [JsonProperty("peer")]
            public string? Peer { get; set; }

            [JsonProperty("proxy_port")]
            public int ProxyPort { get; set; }

            [JsonProperty("proxy_port_v6")]
            public int ProxyPortV6 { get; set; }

            [JsonProperty("sock_port")]
            public int SockPort { get; set; }

            [JsonProperty("sock_port_v6")]
            public int SockPortV6 { get; set; }

            [JsonProperty("system")]
            public string? System { get; set; }

            [JsonProperty("public_ip")]
            public string? PublicIp { get; set; }

            [JsonProperty("public_ip_v6")]
            public string? PublicIpV6 { get; set; }

            [JsonProperty("resetting")]
            public bool Resetting { get; set; }

            [JsonProperty("ppp")]
            public string? Ppp { get; set; }

            [JsonProperty("ppp_tty")]
            public string? PppTty { get; set; }


            public string GetProxy() => $"http://{System}:{ProxyPort}";
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
