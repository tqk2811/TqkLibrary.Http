using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TqkLibrary.Http.Api.Phone
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class RentCodeApi : BaseApi
    {
        private const string EndPoint = "https://api.rentcode.net/api/v2/";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApiKey"></param>
        public RentCodeApi(string ApiKey) : base(ApiKey)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="RentCodeException"></exception>
        /// <returns></returns>
        public Task<RentCodeResult> Request(
          int? MaximumSms = null,
          bool? AllowVoiceSms = null,
          RentCodeNetworkProvider? networkProvider = null,
          RentCodeServiceProviderId? serviceProviderId = null,
          CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "order/request")
                .WithParam("apiKey", ApiKey)
                .WithParam("ServiceProviderId", (int)serviceProviderId)
                .WithParamIfNotNull("NetworkProvider", (int?)networkProvider)
                .WithParamIfNotNull("MaximumSms", MaximumSms)
                .WithParamIfNotNull("AllowVoiceSms", AllowVoiceSms))
            .ExecuteAsync<RentCodeResult>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<RentCodeResult> RequestHolding(
          RentCodeNetworkProvider networkProvider,
          int Duration = 300,
          int Unit = 1,
          CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "order/request-holding")
                .WithParam("apiKey", ApiKey)
                .WithParam("Duration", Duration)
                .WithParam("Unit", Unit)
                .WithParam("NetworkProvider", (int)networkProvider))
            .ExecuteAsync<RentCodeResult>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<RentCodeCheckOrderResults> Check(RentCodeResult rentCodeResult, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "order", rentCodeResult.Id, "check").WithParam("apiKey", ApiKey))
            .ExecuteAsync<RentCodeCheckOrderResults>(cancellationToken);
    }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class RentCodeException : Exception
    {
        internal RentCodeException(string Message)
        {
        }
    }
    public enum RentCodeServiceProviderId : int
    {
        None = 0,
        Facebook = 3,
    }
    public sealed class RentCodeSmsMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("time")]
        public DateTime Time { get; set; }
    }
    public sealed class RentCodeResult
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("success")]
        public bool? Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public override string ToString()
        {
            return $"Id: {Id},Success: {Success},Message: {Message}";
        }
    }
    public enum RentCodeNetworkProvider : int
    {
        Viettel = 1,
        VinaPhone = 2,
        MobilePhone = 3,
        VietnamMobile = 4,
        Cambodia = 5,
        ITelecom = 6
    }
    public sealed class RentCodeCheckOrderResults
    {
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("messages")]
        public List<RentCodeSmsMessage> Messages { get; set; }

        public override string ToString()
        {
            return $"Success: {Success}, PhoneNumberApi: {PhoneNumber}, Message: {Message}";
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}