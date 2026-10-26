using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Proxy.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class TmProxyApi : BaseApi
    {
        const string EndPoint = "https://tmproxy.com/api/proxy/";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApiKey"></param>
        public TmProxyApi(string ApiKey) : base(ApiKey)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TMProxyResponse<TMProxyStatResponse>> Stats(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlPostJson(EndPoint + "stats", new { api_key = ApiKey })
            .ExecuteAsync<TMProxyResponse<TMProxyStatResponse>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TMProxyResponse<TMProxyProxyResponse>> GetCurrentProxy(CancellationToken cancellationToken = default)
             => Build()
            .WithUrlPostJson(EndPoint + "get-current-proxy", new { api_key = ApiKey })
            .ExecuteAsync<TMProxyResponse<TMProxyProxyResponse>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TMProxyResponse<TMProxyProxyResponse>> GetNewProxy(int id_location = 0, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlPostJson(EndPoint + "get-new-proxy", new { api_key = ApiKey, id_location })
            .ExecuteAsync<TMProxyResponse<TMProxyProxyResponse>>(cancellationToken);

    }



#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TMProxyStatResponse
    {
        public int? id { get; set; }
        public DateTime? expired_at { get; set; }
        public string plan { get; set; }
        public int? price_per_day { get; set; }
        public int? timeout { get; set; }
        public int? base_next_request { get; set; }
        public string api_key { get; set; }
        public string note { get; set; }
        public int? max_ip_per_day { get; set; }
        public int? ip_used_today { get; set; }
    }
    public class TMProxyResponse<T>
    {
        public int code { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }
    public class TMProxyProxyResponse
    {
        public string ip_allow { get; set; }
        public string location_name { get; set; }
        public string socks5 { get; set; }
        public string https { get; set; }
        public int? timeout { get; set; }
        public int? next_request { get; set; }
        public string expired_at { get; set; }

        [JsonIgnore]
        public DateTime? ExpiredAt
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(expired_at) && DateTime.TryParseExact(
                    expired_at,
                    "HH:mm:ss dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime startDate))
                {
                    return startDate;
                }
                return null;
            }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
