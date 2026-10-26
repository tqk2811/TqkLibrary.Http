using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Phone
{
    /// <summary>
    /// https://simthue.com/vi/api/index
    /// </summary>
    [Obsolete("Dead service")]
    public sealed class SimThueApi : BaseApi
    {
        private const string EndPoint = "http://api.simthue.com";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApiKey"></param>
        public SimThueApi(string ApiKey) : base(ApiKey)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<SimThueBalanceResult> GetBalance(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "balance").WithParam("key", ApiKey))
            .ExecuteAsync<SimThueBalanceResult>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<SimThueServicesResult> GetAvailableServices(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "service").WithParam("key", ApiKey))
            .ExecuteAsync<SimThueServicesResult>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceResult"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<SimThueRequestResult> CreateRequest(SimThueServiceResult serviceResult, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "create").WithParam("key", ApiKey).WithParam("service_id",serviceResult.Id))
            .ExecuteAsync<SimThueRequestResult>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createResult"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<SimThueCheckResult> CheckRequest(SimThueRequestResult createResult, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "check").WithParam("key", ApiKey).WithParam("id", createResult.Id))
            .ExecuteAsync<SimThueCheckResult>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createResult"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<SimThueRequestResult> CancelRequest(SimThueRequestResult createResult, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "cancel").WithParam("key", ApiKey).WithParam("id", createResult.Id))
            .ExecuteAsync<SimThueRequestResult>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createResult"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<SimThueRequestResult> FinishRequest(SimThueRequestResult createResult, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "finish").WithParam("key", ApiKey).WithParam("id", createResult.Id))
            .ExecuteAsync<SimThueRequestResult>(cancellationToken);
    }



#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class SimThueServicesResult : SimThueBaseResult
    {
        [JsonProperty("services")]
        public List<SimThueServiceResult> Services { get; set; }
    }

    public class SimThueServiceResult
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
    }
    public class SimThueRequestResult : SimThueBalanceResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
    public class SimThueCheckResult : SimThueBaseResult
    {
        [JsonProperty("number")]
        public int? Number { get; set; }

        [JsonProperty("timeleft")]
        public int? TimeLeft { get; set; }

        /// <summary>
        /// example: sms: ["1612133550|Google|G-195829 is your Google verification code.", "1512113580|Google|G-120094 is your Google verification code."]
        /// </summary>
        [JsonProperty("sms")]
        public List<string> Sms { get; set; }
    }
    public abstract class SimThueBaseResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
    public class SimThueBalanceResult : SimThueBaseResult
    {
        [JsonProperty("balance")]
        public double? Balance { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}