using System;
using System.Collections.Generic;

namespace TqkLibrary.Http.Api.Other.Social.Facebook
{
    internal class FacebookToken_
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    public class FacebookToken
    {
        public FacebookToken()
        {
        }

        internal FacebookToken(FacebookToken_ token)
        {
            this.access_token = token.access_token;
            this.token_type = token.token_type;
            this.ExpiresAt = DateTime.Now.AddSeconds(token.expires_in);
        }

        public string access_token { get; set; }
        public string token_type { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class FacebookUser
    {
        public string name { get; set; }
        public string id { get; set; }
    }

    public class DataPages
    {
        public List<PageData> data { get; set; }
    }

    public class PageData
    {
        public string access_token { get; set; }
        public string category { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public List<string> tasks { get; set; }
    }
}