using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Proxy.Services
{
    
    /// <summary>
    /// http://proxy.tinsoftsv.com/api/document_vi.php
    /// </summary>
    public sealed class TinsoftProxyApi : BaseApi
    {
        internal const string EndPoint = "http://proxy.tinsoftsv.com/api";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApiKey"></param>
        public TinsoftProxyApi(string ApiKey) : base(ApiKey)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<TinsoftProxyProxyResult> ChangeProxy(int location = 0, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint + "/changeProxy.php").WithParam("key", ApiKey).WithParam("location", location))
            .ExecuteAsync<TinsoftProxyProxyResult>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TinsoftProxyKeyInfo> GetKeyInfo(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint + "/getKeyInfo.php").WithParam("key", ApiKey))
            .ExecuteAsync<TinsoftProxyKeyInfo>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TinsoftProxyKeyInfo> DeleteKey(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint + "/deleteKey.php").WithParam("key", ApiKey))
            .ExecuteAsync<TinsoftProxyKeyInfo>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TinsoftProxyLocationResult> GetLocations(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint + "/getLocations.php"))
            .ExecuteAsync<TinsoftProxyLocationResult>(cancellationToken);
    }




    /// <summary>
    /// 
    /// </summary>
    public class TinsoftProxyUserKeyDataInfo : TinsoftProxyBaseResult
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }


        /// <summary>
        /// 
        /// </summary>

        [JsonProperty("date_expired")]
        public DateTime DateExpired { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [JsonProperty("isVip")]
        public int IsVip { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public enum TinsoftProxyVip
    {
        /// <summary>
        /// key thường
        /// </summary>
        Nomal = 0,

        /// <summary>
        /// key vip ko timeout
        /// </summary>
        NonTimeout = 1,

        /// <summary>
        /// key vip dùng nhanh (1phút được đổi ip)
        /// </summary>
        Fast = 2
    }
    /// <summary>
    /// 
    /// </summary>
    public class TinsoftProxyProxyResult : TinsoftProxyBaseResult
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("proxy")]
        public string Proxy { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [JsonProperty("next_change")]
        public int NextChange { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [JsonProperty("timeout")]
        public int Timeout { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TinsoftProxyTinsoftOrderDataResult : TinsoftProxyBaseResult
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("date_expired")]
        public DateTime DateExpired { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("isVip")]
        public int IsVip { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TinsoftProxyLocationResult : TinsoftProxyBaseResult
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("data")]
        public List<TinsoftProxyLocation> Data { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TinsoftProxyLocation
    {
        /// <summary>
        /// 
        /// </summary>
        public int? location { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public override string ToString() => $"location: {location}, name: {name}";
    }
    /// <summary>
    /// 
    /// </summary>
    public class TinsoftProxyKeyInfo : TinsoftProxyBaseResult
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("date_expired")]
        public DateTime DateExpired { get; set; }
        /// <summary>
        /// 
        /// </summary>

        [JsonProperty("isVip")]
        public int IsVip { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TinsoftProxyBaseResult
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
        /// <summary>
        /// 
        /// </summary>

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}