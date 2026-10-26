using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace TqkLibrary.Http.Api.Mail.Services.TempMails
{
    /// <summary>
    /// 
    /// </summary>
    public class TempMailOrgEndPoint
    {
        /// <summary>
        /// 
        /// </summary>
        public Uri Uri { get; }
        internal TempMailOrgEndPoint(Uri uri)
        {
            this.Uri = uri;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class TempMailOrg : BaseApi
    {
        /// <summary>
        /// https://mob2.temp-mail.org
        /// </summary>
        public static readonly TempMailOrgEndPoint Mob2 = new TempMailOrgEndPoint(new Uri("https://mob2.temp-mail.org"));
        /// <summary>
        /// https://web2.temp-mail.org
        /// </summary>
        public static readonly TempMailOrgEndPoint Web2 = new TempMailOrgEndPoint(new Uri("https://web2.temp-mail.org"));

        TempMailOrgEndPoint _EndPoint = Mob2;
        /// <summary>
        /// 
        /// </summary>
        public TempMailOrgEndPoint EndPoint
        {
            get { return _EndPoint; }
            set
            {
                if (value == null) throw new NullReferenceException(nameof(EndPoint));
                _EndPoint = value;
            }
        }
        const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36 Edg/90.0.818.62";
        
        /// <summary>
        /// 
        /// </summary>
        public TempMailOrg() : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TempMailOrgToken> InitToken(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlPostJson(new UrlBuilder(EndPoint.Uri.AbsoluteUri, "mailbox"), new object())
            .WithHeader("User-Agent", UserAgent)
            .WithHeader("Referer", "https://temp-mail.org/")
            //.WithHeader("Accept", "application/json")
            //.WithHeader("Accept-Encoding", "deflate")
            .ExecuteAsync<TempMailOrgToken>(cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TempMailOrgMailBox> Messages(TempMailOrgToken token, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint.Uri.AbsoluteUri, "messages"))
            .WithHeader("Authorization", token.Token)
            .WithHeader("Referer", "https://temp-mail.org/")
            .WithHeader("User-Agent", UserAgent)
            //.WithHeader("Accept-Encoding", "deflate")
            .ExecuteAsync<TempMailOrgMailBox>(cancellationToken);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TempMailOrgMessageData> MessageData(TempMailOrgToken token, TempMailOrgMessageReView messageReView, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint.Uri.AbsoluteUri, "messages", messageReView.Id))
            .WithHeader("Authorization", token.Token)
            .WithHeader("Referer", "https://temp-mail.org/")
            .WithHeader("User-Agent", UserAgent)
            //.WithHeader("Accept-Encoding", "deflate")
            .ExecuteAsync<TempMailOrgMessageData>(cancellationToken);

    }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TempMailOrgToken
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("mailbox")]
        public string MailBox { get; set; }
    }

    public class TempMailOrgMailBox
    {
        [JsonProperty("mailbox")]
        public string MailBox { get; set; }

        [JsonProperty("messages")]
        public List<TempMailOrgMessageReView> Messages { get; set; }
    }

    public class TempMailOrgMessageReView
    {
        [JsonProperty("_id")]
        public string Id { get; set; }


        [JsonProperty("receivedAt")]
        public long ReceivedAt { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }


        [JsonProperty("subject")]
        public string Subject { get; set; }


        [JsonProperty("bodyPreview")]
        public string BodyPreview { get; set; }


        [JsonProperty("attachmentsCount")]
        public int AttachmentsCount { get; set; }
    }
    public class TempMailOrgMessageData : TempMailOrgMessageReView
    {
        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("mailbox")]
        public string Mailbox { get; set; }

        [JsonProperty("bodyHtml")]
        public string BodyHtml { get; set; }

        //public object[] attachments { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
