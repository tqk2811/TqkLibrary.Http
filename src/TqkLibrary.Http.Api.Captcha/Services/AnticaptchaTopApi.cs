using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Captcha.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AnticaptchaTopApi : BaseApi
    {
        readonly string _Endpoint = "https://anticaptcha.top";
        /// <summary>
        /// 
        /// </summary>
        public AnticaptchaTopApi(string apiKey) : base(apiKey)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageToTextOption"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ImageToTextResponse> ImageToTextAsync(ImageToTextOption imageToTextOption, CancellationToken cancellationToken = default)
            => Build()
                .WithUrlPostJson(new UrlBuilder(_Endpoint, "api", "captcha"), imageToTextOption.ToJsonObject(ApiKey))
                .ExecuteAsync<ImageToTextResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="googleKey"></param>
        /// <param name="pageUrl"></param>
        /// <param name="isInvisible"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<RecaptchaV2Response> RecaptchaV2Async(string googleKey, string pageUrl, bool isInvisible = false, CancellationToken cancellationToken = default)
            => Build()
                .WithUrlPostJson(new UrlBuilder(_Endpoint, "api", "captcha"), new RecaptchaV2Request()
                {
                    ApiKey = ApiKey,
                    Type = 11,
                    PageUrl = pageUrl,
                    GoogleSiteKey = googleKey,
                    Invisible = isInvisible ? 1 : 0,
                })
                .ExecuteAsync<RecaptchaV2Response>(cancellationToken);



#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public class BaseResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }
            [JsonProperty("message")]
            public string? Message { get; set; }
        }
        public class ImageToTextResponse : BaseResponse
        {
            [JsonProperty("captcha")]
            public string? Captcha { get; set; }
            [JsonProperty("base64img")]
            public string? Base64img { get; set; }
        }
        public class ImageToTextOption
        {
            public ImageToTextOption(Uri uri)
            {
                this.Image = uri.ToString();
            }
            public ImageToTextOption(byte[] buffer)
            {
                this.Image = Convert.ToBase64String(buffer);
            }
            public string Image { get; }
            public ImageType ImageType { get; set; } = ImageType.Any;
            public bool IsCalc { get; set; } = false;
            public bool IsNumeric { get; set; } = false;
            public bool IsCasesensitive { get; set; } = false;

            public static implicit operator ImageToTextOption(byte[] buffer) => new ImageToTextOption(buffer);

            internal JsonImageToTextOption ToJsonObject(string apiKey)
            {
                return new JsonImageToTextOption()
                {
                    ApiKey = apiKey,
                    Image = Image,
                    ImageType = ImageType,
                    Calc = IsCalc ? 1 : null,
                    Numeric = IsNumeric ? 1 : null,
                    Casesensitive = IsCasesensitive ? 1 : null,
                };
            }
            internal class JsonImageToTextOption
            {
                [JsonProperty("apiKey")]
                public string? ApiKey { get; set; }
                [JsonProperty("img")]
                public string? Image { get; set; }
                [JsonProperty("type")]
                public ImageType ImageType { get; set; }
                [JsonProperty("calc")]
                public int? Calc { get; set; }
                [JsonProperty("numeric")]
                public int? Numeric { get; set; }
                [JsonProperty("casesensitive")]
                public int? Casesensitive { get; set; }
            }
        }

        public enum ImageType
        {
            Any = 0,
            MyViettel = 1,
            MicroSoft = 2,
            MyVina = 3,
            MyVnpt = 3,
            BotDetect = 5,
            FACEBOOK = 6,
            GARENA = 7,
            NRO = 8,
            Vietcombank = 9,
            AMAZONE = 10,
            Any14 = 14,
            MBBank = 18,
            VietinBank = 19,
            MAJESTIC = 20,
        }


        class RecaptchaV2Request
        {
            [JsonProperty("apikey")]
            public string? ApiKey { get; set; }

            [JsonProperty("type")]
            public int Type { get; set; }

            [JsonProperty("googlesitekey")]
            public string? GoogleSiteKey { get; set; }

            [JsonProperty("pageurl")]
            public string? PageUrl { get; set; }

            [JsonProperty("invi")]
            public int Invisible { get; set; } = 0;
        }
        public class RecaptchaV2Response
        {
            [JsonProperty("success")]
            public bool? Success { get; set; }

            [JsonProperty("message")]
            public string? Message { get; set; }

            [JsonProperty("captcha")]
            public string? Captcha { get; set; }

            [JsonProperty("base64img")]
            public string? Base64Img { get; set; }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
