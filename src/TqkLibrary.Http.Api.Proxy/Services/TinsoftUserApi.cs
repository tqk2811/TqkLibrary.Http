using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Proxy.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class TinsoftUserApi : BaseApi
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserApiKey"></param>
        public TinsoftUserApi(string UserApiKey) : base(UserApiKey)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TinsoftProxyUserKeyInfo> GetUserKeys(CancellationToken cancellationToken = default)
             => Build()
            .WithUrlGet(new UrlBuilder(TinsoftProxyApi.EndPoint + "/getUserKeys.php").WithParam("key", ApiKey))
            .ExecuteAsync<TinsoftProxyUserKeyInfo>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TinsoftProxyUserInfo> GetUserInfo(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(TinsoftProxyApi.EndPoint + "/getUserInfo.php").WithParam("key", ApiKey))
            .ExecuteAsync<TinsoftProxyUserInfo>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="dateTime"></param>
        /// <param name="tinsoftVip"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TinsoftProxyOrderResult> OrderKeys(int quantity, DateTime dateTime, TinsoftProxyVip tinsoftVip, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(TinsoftProxyApi.EndPoint + "/orderKeys.php")
                .WithParam("key", ApiKey)
                .WithParam("quantity", quantity)
                .WithParam("days", $"{dateTime:dd-MM-yyyy HH:mm:ss}")
                .WithParam("vip", (int)tinsoftVip))
            .ExecuteAsync<TinsoftProxyOrderResult>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="proxyKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TinsoftProxyBaseResult> ExtendKey(DateTime dateTime, string proxyKey, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(TinsoftProxyApi.EndPoint + "/orderKeys.php")
                .WithParam("key", ApiKey)
                .WithParam("days", $"{dateTime:dd-MM-yyyy HH:mm:ss}")
                .WithParam("proxy_key", proxyKey))
            .ExecuteAsync<TinsoftProxyBaseResult>(cancellationToken);
    }



#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TinsoftProxyUserInfo : TinsoftProxyBaseResult
    {
        [JsonProperty("max_key")]
        public int MaxKey { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }

        [JsonProperty("paid")]
        public double Paid { get; set; }

        [JsonProperty("wallet_key")]
        public string WalletKey { get; set; }
    }
    public class TinsoftProxyUserKeyInfo : TinsoftProxyBaseResult
    {
        [JsonProperty("data")]
        public List<TinsoftProxyUserKeyDataInfo> Data { get; set; }
    }
    public class TinsoftProxyOrderResult : TinsoftProxyBaseResult
    {
        [JsonProperty("data")]
        public List<TinsoftProxyTinsoftOrderDataResult> Data { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}