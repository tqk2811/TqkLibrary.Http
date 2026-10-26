namespace TqkLibrary.Http.Api.Captcha.Wrapper.Classes
{
    public class HcaptchaDataRequest
    {
        public required string SiteKey { get; init; }
        public required string PageUrl { get; init; }
        public string? Domain { get; set; }
        public string? Data { get; set; }
        public string? Cookies { get; set; }
        /// <summary>
        /// LOGIN:PASS@IP:PORT
        /// </summary>
        public string? Proxy { get; set; }
    }
}
