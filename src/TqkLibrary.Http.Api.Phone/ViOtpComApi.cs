using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TqkLibrary.Utils;

namespace TqkLibrary.Http.Api.Phone
{
    /// <summary>
    /// https://viotp.com/Account/ApiDocument
    /// </summary>
    public class ViOtpComApi : BaseApi
    {
        const string EndPoint = "https://api.viotp.com";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public ViOtpComApi(string apiKey) : base(apiKey)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ViOtpComResponse<ViOtpComBalance>> GetBalance(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "users", "balance")
                .WithParam("token", ApiKey))
            .ExecuteAsync<ViOtpComResponse<ViOtpComBalance>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ViOtpComResponse<List<ViOtpComNetwork>>> GetNetworks(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "networks", "get")
                .WithParam("token", ApiKey))
            .ExecuteAsync<ViOtpComResponse<List<ViOtpComNetwork>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ViOtpComResponse<List<ViOtpComService>>> GetServices(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "service", "get")
                .WithParam("token", ApiKey!))
            .ExecuteAsync<ViOtpComResponse<List<ViOtpComService>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<ViOtpComResponse<ViOtpComSession>> RequestRent(
            ViOtpComService service,
            IEnumerable<ViOtpComNetwork>? networks = null,
            IEnumerable<string>? prefix = null,
            IEnumerable<string>? exceptPrefix = null,
            CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "request", "get")
                .WithParam("token", ApiKey!)
                .WithParam("serviceId", service.Id)
                .WithParamIfNotNull("networks", networks?.Select(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x)).Join("|"))
                .WithParamIfNotNull("prefix", prefix?.Join("|"))
                .WithParamIfNotNull("exceptPrefix", exceptPrefix?.Join("|")))
            .ExecuteAsync<ViOtpComResponse<ViOtpComSession>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="number"> số điện thoại cần thuê lại có bao gồm số 0 ở đầu</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ViOtpComResponse<ViOtpComSession>> RequestReRent(
            ViOtpComService service,
            string number,
            CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "request", "get")
                .WithParam("token", ApiKey!)
                .WithParam("serviceId", service.Id)
                .WithParam("number", number))
            .ExecuteAsync<ViOtpComResponse<ViOtpComSession>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ViOtpComResponse<ViOtpComSessionGet>> SessionGet(
            ViOtpComSession session,
            CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "session", "get")
                .WithParam("token", ApiKey!)
                .WithParam("service", session.RequestId))
            .ExecuteAsync<ViOtpComResponse<ViOtpComSessionGet>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="status"></param>
        /// <param name="limit"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ViOtpComResponse<ViOtpComSessionGet>> SessionHistory(
            ViOtpComService? service = null,
            int status = 2,
            int limit = 100,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "session", "history")
                .WithParam("token", ApiKey!)
                .WithParamIfNotNull("service", service?.Id)
                .WithParamIfNotNull("status", status)
                .WithParamIfNotNull("limit", limit)
                .WithParamIfNotNull("fromDate", fromDate?.ToString("yyyy-MM-dd"))
                .WithParamIfNotNull("toDate",  toDate?.ToString("yyyy-MM-dd")))
            .ExecuteAsync<ViOtpComResponse<ViOtpComSessionGet>>(cancellationToken);
    }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public class ViOtpComResponse<T>
    {
        [JsonProperty("status_code")]
        public int StatusCode { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }
    public class ViOtpComBalance
    {
        [JsonProperty("balance")]
        public double Balance { get; set; }
    }
    public class ViOtpComNetwork
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
    public class ViOtpComService
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
    }
    public class ViOtpComSession
    {
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }

        [JsonProperty("request_id")]
        public int RequestId { get; set; }
    }
    public class ViOtpComSessionGet
    {
        public int ID { get; set; }
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public int Status { get; set; }
        public double Price { get; set; }
        public string Phone { get; set; }
        public string SmsContent { get; set; }
        public string IsSound { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Code { get; set; }
    }



#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
