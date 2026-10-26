using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Captcha.Services
{
    public class OmoCaptcaComApi : BaseApi
    {
        const string _EndPoint = "https://omocaptcha.com/v2";
        public OmoCaptcaComApi(string apiKey) : base(apiKey)
        {

        }

        public class BaseResponse
        {
            [JsonProperty("errorId")]
            public int ErrorId { get; set; }

            [JsonProperty("errorCode")]
            public string? ErrorCode { get; set; }

            [JsonProperty("errorDescription")]
            public string? ErrorDescription { get; set; }
        }



        public class BalanceResponse : BaseResponse
        {
            [JsonProperty("balance")]
            public double Balance { get; set; }

            [JsonProperty("quantity")]
            public int Quantity { get; set; }
        }
        public Task<BalanceResponse> GetBalanceAsync(CancellationToken cancellationToken = default)
        {
            return Build()
                .WithUrlPostJson(new UrlBuilder(_EndPoint, "getBalance"), new { clientKey = ApiKey })
                .ExecuteAsync<BalanceResponse>(cancellationToken);
        }
    }
}
