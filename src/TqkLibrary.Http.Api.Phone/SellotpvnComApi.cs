using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Phone
{
    public class SellotpvnComApi : BaseApi
    {
        public enum NetworkProvider
        {
            Viettel,
            Mobifone,
            Vinaphone,
            Vietnamobile,
            Itel
        }
        public class Serivce
        {
            public Serivce(string serviceName)
            {
                if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException(nameof(serviceName));
                this.ServiceName = serviceName;
            }
            public string ServiceName { get; }

            public static readonly Serivce DOSI = new("dosi");
            public static readonly Serivce Facebook = new("facebook");
            public static readonly Serivce Telegram = new("telegram");
            public static readonly Serivce Shopee = new("shopee");
            public static readonly Serivce Hotmail = new("hotmail");
            public static readonly Serivce Gmail = new("gmail");
            public static readonly Serivce Zalo = new("zalo");
            public static readonly Serivce KokPlay = new("kokplay");
            public static readonly Serivce Wechat = new("wechat");
            public static readonly Serivce Onjoyride = new("joyride");
            public static readonly Serivce Garena = new("garena");
            public static readonly Serivce Lazada = new("lazada");
            public static readonly Serivce SenDo = new("sendo");
            public static readonly Serivce BBMeta = new("bbmeta");
            public static readonly Serivce Instagram = new("insg");
            public static readonly Serivce Discord = new("discord");
            public static readonly Serivce TikTok = new("tiktok");
            public static readonly Serivce Shbet = new("shbet");
            public static readonly Serivce HappyPig = new("happy");
        }

        public class Response
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("phoneNumber")]
            public string PhoneNumber { get; set; }

            [JsonProperty("serviceName")]
            public string ServiceName { get; set; }

            [JsonProperty("content")]
            public string Content { get; set; }

            [JsonProperty("otp")]
            public string Otp { get; set; }

            [JsonProperty("status")]
            public Status Status { get; set; }

            [JsonProperty("createdAt")]
            public string CreatedAt { get; set; }

            [JsonProperty("msg")]
            public string Message { get; set; }
        }
        public enum Status
        {
            False = 0,
            Failed = 0,
            Successed,
            Pending
        }


        public class ApiEndPoint
        {
            public ApiEndPoint(Uri uri)
            {
                this.Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            }
            public Uri Uri { get; }

            public static readonly ApiEndPoint SellotpVn = new ApiEndPoint(new Uri("https://sellotpvn.com/api/gsm"));
            public static readonly ApiEndPoint NumberotpCo = new ApiEndPoint(new Uri("https://numberotp.co/api/gsm"));
        }


        readonly ApiEndPoint apiEndPoint;
        public SellotpvnComApi(ApiEndPoint apiEndPoint, string token) : base(token)
        {
            this.apiEndPoint = apiEndPoint ?? throw new ArgumentNullException(nameof(apiEndPoint));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serivce"></param>
        /// <param name="networkProvider">Null for random</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task<Response> Order(
            Serivce serivce,
            NetworkProvider? networkProvider = null,
            CancellationToken cancellationToken = default)
        {
            if (serivce is null) throw new ArgumentNullException(nameof(serivce));

            return Build()
                .WithUrlGet(new UrlBuilder(apiEndPoint.Uri, "order", serivce.ServiceName, ApiKey)
                                .WithParamIfNotNull("provider", networkProvider))
                .ExecuteAsync<Response>(cancellationToken);
        }

        public virtual Task<Response> GetOrder(
            Response response,
            CancellationToken cancellationToken = default)
        {
            if (response is null) throw new ArgumentNullException(nameof(response));
            return Build()
                .WithUrlGet(new UrlBuilder(apiEndPoint.Uri, "get-order", response.Id, ApiKey))
                .ExecuteAsync<Response>(cancellationToken);
        }
    }
}
