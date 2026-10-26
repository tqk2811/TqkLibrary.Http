using Newtonsoft.Json;
using System;

namespace TqkLibrary.Http.Api.Captcha.Wrapper.Classes
{
    public class RecaptchaV2DataRequest
    {
        public required string DataSiteKey { get; init; }
        public required string PageUrl { get; init; }


        public string? Proxy { get; set; }
        public string? ProxyType { get; set; }
        public string? UserAgent { get; set; }
        public string? DataS { get; set; }
        public string? Domain { get; set; }
        public bool? IsInvisible { get; set; }
        public bool? IsEnterprise { get; set; }
        public string? Cookies { get; set; }
    }
}
