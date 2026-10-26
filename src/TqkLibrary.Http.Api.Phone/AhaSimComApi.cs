using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace TqkLibrary.Http.Api.Phone
{
    /// <summary>
    /// http://ahasim.com/user/api
    /// </summary>
    public class AhaSimComApi : BaseApi
    {
        const string EndPoint = "http://ahasim.com/api";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public AhaSimComApi(string apiKey) : base(apiKey)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AhaSimComResponse<AhaSimComBalance>> Balance(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "user", "balance").WithParam("token", ApiKey))
            .ExecuteAsync<AhaSimComResponse<AhaSimComBalance>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AhaSimComResponse<List<AhaSimComNetwork>>> NetworkList(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "network", "list").WithParam("token", ApiKey))
            .ExecuteAsync<AhaSimComResponse<List<AhaSimComNetwork>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AhaSimComResponse<List<AhaSimComService>>> ServiceList(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "service", "list").WithParam("token", ApiKey))
            .ExecuteAsync<AhaSimComResponse<List<AhaSimComService>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service">Mã dịch vụ lấy ở <see cref="ServiceList"/></param>
        /// <param name="networks">ID nhà mạng lấy ở <see cref="NetworkList"/></param>
        /// <param name="prefixs">Đầu số muốn lấy số.Bao gồm các giá trị đầu số của các nhà mạng, dùng dấu phẩy nếu muốn lấy nhiều đầu số</param>
        /// <param name="except_prefixs"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<AhaSimComResponse<AhaSimComSession>> PhoneNewSession(
            AhaSimComService service,
            IEnumerable<AhaSimComNetwork> networks = null,
            IEnumerable<string> prefixs = null,
            IEnumerable<string> except_prefixs = null,
            CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "phone", "new-session").WithParam("token", ApiKey)
                .WithParam("service", service.Id)
                .WithParamIfNotNull("network", networks == null ? null : string.Join(",", networks.Select(x => x.Id)))
                .WithParamIfNotNull("prefix",  prefixs == null ? null : string.Join(",", prefixs))
                .WithParamIfNotNull("except_prefix", except_prefixs == null ? null : string.Join(",", except_prefixs)))
            .ExecuteAsync<AhaSimComResponse<AhaSimComSession>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service">Mã dịch vụ lấy ở <see cref="ServiceList"/></param>
        /// <param name="number">Số điện thoại muốn mua lại.Dùng trong trường hợp ứng dụng yêu cầu xác thực nhiều lần</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<AhaSimComResponse<AhaSimComSession>> PhoneNewSessionReBuy(
            AhaSimComService service,
            string number,
            CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "phone", "new-session").WithParam("token", ApiKey)
                .WithParam("service", service.Id)
                .WithParamIfNotNull("number", number))
            .ExecuteAsync<AhaSimComResponse<AhaSimComSession>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<AhaSimComResponse<AhaSimComOtp>> SessionGetOtp(AhaSimComSession session, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "session", session.Session, "get-otp").WithParam("token", ApiKey))
            .ExecuteAsync<AhaSimComResponse<AhaSimComOtp>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<AhaSimComResponse<AhaSimComSessionCancel>> SessionCancel(AhaSimComSession session, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "session", "cancel")
                .WithParam("token", ApiKey)
                .WithParam("session", session.Session))
            .ExecuteAsync<AhaSimComResponse<AhaSimComSessionCancel>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AhaSimComResponse<List<AhaSimComHistory>>> ServiceHistory(
            string number = null,
            int? service_id = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "service", "history")
                .WithParam("token", ApiKey)
                .WithParamIfNotNull("number", number)
                .WithParamIfNotNull("service_id", service_id)
                .WithParamIfNotNull("fromDate", fromDate)
                .WithParamIfNotNull("toDate", toDate))
            .ExecuteAsync<AhaSimComResponse<List<AhaSimComHistory>>>(cancellationToken);
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public class AhaSimComBalance
    {
        [JsonProperty("balance")]
        public double Balance { get; set; }
    }

    public class AhaSimComNetwork
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class AhaSimComService
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
    }

    public class AhaSimComSession
    {
        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }
    }

    public class AhaSimComOtp
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("service_id")]
        public int ServiceId { get; set; }

        [JsonProperty("service_name")]
        public string ServiceName { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("message")]
        public AhaSimComMessage Message { get; set; }
    }
    public class AhaSimComMessage
    {
        [JsonProperty("network")]
        public string Network { get; set; }


        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("otp")]
        public string Otp { get; set; }
    }

    public class AhaSimComSessionCancel
    {
        [JsonProperty("refund")]
        public double Refund { get; set; }
    }


    public class AhaSimComHistory
    {
        [JsonProperty("service_id")]
        public int ServiceId { get; set; }

        [JsonProperty("cost")]
        public double Cost { get; set; }

        [JsonProperty("session")]
        public string Session { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("expired_at")]
        public DateTime? ExpiredAt { get; set; }

        [JsonProperty("done_at")]
        public DateTime? DoneAt { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("message")]
        public AhaSimComMessage Message { get; set; }
    }

    public class AhaSimComResponse<T>
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
