using Newtonsoft.Json;
using System;

namespace TqkLibrary.Http.Api.Mail.OutlookGraphApi.Classes
{
    public class AuthenticationResponse
    {
        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        [JsonProperty("scope")]
        public string? Scope { get; set; }

        [JsonProperty("expires_in")]
        public int? ExpiresIn { get; set; }

        [JsonProperty("ext_expires_in")]
        public int? ExtExpiresIn { get; set; }

        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }


        [JsonIgnore]
        public DateTime ObjectCreateTime = DateTime.UtcNow;
        [JsonIgnore]
        public bool IsExpired
        {
            get
            {
                int expiresIn = this.ExpiresIn.HasValue ? this.ExpiresIn.Value - 10 : 0;
                return DateTime.UtcNow >= ObjectCreateTime.AddSeconds(expiresIn);
            }
        }
    }
}
