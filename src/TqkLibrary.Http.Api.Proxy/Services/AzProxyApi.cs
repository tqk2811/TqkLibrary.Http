using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
//https://documenter.getpostman.com/view/11108442/TzecCQHG
namespace TqkLibrary.Http.Api.Proxy.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AzProxyApi : BaseApi
    {
        const string EndPoint = "http://proxy.shoplike.vn/Api/";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public AzProxyApi(string apiKey) : base(apiKey)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="provider"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<AzProxyResponse> GetNewProxy(AzProxyLocation? location = null, AzProxyProvider? provider = null, CancellationToken cancellationToken = default)
            => base.Build()
                .WithUrlGet(
                    new UrlBuilder($"{EndPoint}getNewProxy")
                        .WithParam("access_token", ApiKey!)
                        .WithParamIfNotNull("location", location)
                        .WithParamIfNotNull("provider", provider))
                .ExecuteAsync<AzProxyResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<AzProxyResponse> GetCurrentProxy(CancellationToken cancellationToken = default)
           => base.Build()
               .WithUrlGet(
                   new UrlBuilder($"{EndPoint}getCurrentProxy")
                       .WithParam("access_token", ApiKey!))
               .ExecuteAsync<AzProxyResponse>(cancellationToken);




#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum AzProxyLocation
        {
            qn, hcm, hd, hue, bn
        }
        public enum AzProxyProvider
        {
            vnpt, viettel, fpt
        }
        public class AzProxyResponse
        {
            /// <summary>
            /// error, success
            /// </summary>
            [JsonProperty("status")]
            public string? Status { get; set; }

            [JsonProperty("mess")]
            public string? Message { get; set; }

            [JsonProperty("data")]
            public AzProxyData? Data { get; set; }

            [JsonIgnore]
            public TimeSpan? NextTime
            {
                get
                {
                    if ("error".Contains(Status) && !string.IsNullOrWhiteSpace(Message))
                    {
                        Match match = Regex.Match(Message, @"\d+");
                        if (match.Success) return TimeSpan.FromSeconds(int.Parse(match.Value));
                    }
                    return null;
                }
            }

            [JsonIgnore]
            public bool IsSuccess => "success".Contains(Status);
        }
        public class AzProxyData
        {
            [JsonProperty("location")]
            public string? Location { get; set; }

            [JsonProperty("proxy")]
            public string? Proxy { get; set; }

            [JsonProperty("auth")]
            public string? Auth { get; set; }
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }


}
