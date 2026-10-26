using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TqkLibrary.Http.Api.Mail.Services.TempMails
{
    /// <summary>
    /// https://api.mail.tm/
    /// </summary>
    public class MailTmApi : BaseApi
    {
        const string EndPoint = "https://api.mail.tm";
        /// <summary>
        /// 
        /// </summary>
        public MailTmApi() : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<MailTmCollection<MailTmDomain>> Domains(int page = 1, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "/domains").WithParam("page", page))
            .WithHeader("Accept", "application/ld+json")
            .ExecuteAsync<MailTmCollection<MailTmDomain>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<MailTmDomain> Domain(string id, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "/domains/", id))
            .ExecuteAsync<MailTmDomain>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<MailTmAccountResponse> AccountCreate(MailTmAccount mailTmAccount, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlPostJson(new UrlBuilder(EndPoint, "/accounts"), mailTmAccount)
            .ExecuteAsync<MailTmAccountResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<MailTmAccountResponse> AccountsDelete(MailTmAccountResponse account, MailTmToken token, CancellationToken cancellationToken = default)
            => Build()
            .WithUrl(new UrlBuilder(EndPoint, "/accounts/", account.Id), HttpMethod.Delete)
            .WithHeader("Authorization", $"Bearer {token.Token}")
            .ExecuteAsync<MailTmAccountResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<MailTmToken> Token(MailTmAccount account, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlPostJson(new UrlBuilder(EndPoint, "/token"), account)
            .ExecuteAsync<MailTmToken>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<MailTmAccountResponse> Me(MailTmToken token, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "/me"))
            .WithHeader("Authorization", $"Bearer {token.Token}")
            .ExecuteAsync<MailTmAccountResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<MailTmCollection<MailTmMessage>> Messages(MailTmToken token, int page = 1, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "/messages").WithParam("page", page))
            .WithHeader("Authorization", $"Bearer {token.Token}")
            .WithHeader("Accept", "application/ld+json")
            .ExecuteAsync<MailTmCollection<MailTmMessage>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<MailTmMessageData> Message(MailTmToken token, MailTmMessage message, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "/messages/", message.Id))
            .WithHeader("Authorization", $"Bearer {token.Token}")
            .ExecuteAsync<MailTmMessageData>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<MailTmMessageData> MessageDelete(MailTmToken token, MailTmMessage message, CancellationToken cancellationToken = default)
            => Build()
            .WithUrl(new UrlBuilder(EndPoint, "/messages/", message.Id), HttpMethod.Delete)
            .WithHeader("Authorization", $"Bearer {token.Token}")
            .ExecuteAsync<MailTmMessageData>(cancellationToken);
    }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class MailTmCollection<T>
    {
        [JsonProperty("hydra:member")]
        public List<T> Members { get; set; }


        [JsonProperty("hydra:totalItems")]
        public int TotalItems { get; set; }
    }

    public class MailTmDomain
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class MailTmAccount
    {
        public MailTmAccount()
        {

        }

        public MailTmAccount(string account, MailTmDomain domain, string password)
        {
            this.Address = $"{account}@{domain.Domain}";
            this.Password = password;
        }

        [JsonProperty("address")]
        public string Address { get; set; }


        [JsonProperty("password")]
        public string Password { get; set; }
    }


    public class MailTmAccountResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("quota")]
        public int Quota { get; set; }

        [JsonProperty("used")]
        public int Used { get; set; }

        [JsonProperty("isDisabled")]
        public bool IsDisabled { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class MailTmMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("msgid")]
        public string Msgid { get; set; }

        [JsonProperty("from")]
        public MailTmAddress From { get; set; }

        [JsonProperty("to")]
        public List<MailTmAddress> To { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("intro")]
        public string Intro { get; set; }

        [JsonProperty("seen")]
        public bool Seen { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("hasAttachments")]
        public bool HasAttachments { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class MailTmMessageData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("msgid")]
        public string Msgid { get; set; }

        [JsonProperty("from")]
        public MailTmAddress From { get; set; }

        [JsonProperty("to")]
        public MailTmAddress[] To { get; set; }

        [JsonProperty("cc")]
        public MailTmAddress[] Cc { get; set; }

        [JsonProperty("bcc")]
        public MailTmAddress[] Bcc { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("seen")]
        public bool Seen { get; set; }

        [JsonProperty("flagged")]
        public bool Flagged { get; set; }

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("verifications")]
        public string[] Verifications { get; set; }

        [JsonProperty("retention")]
        public bool Retention { get; set; }

        [JsonProperty("retentionDate")]
        public DateTime RetentionDate { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("html")]
        public string[] Html { get; set; }

        [JsonProperty("hasAttachments")]
        public bool HasAttachments { get; set; }

        [JsonProperty("attachments")]
        public MailTmAttachment[] Attachments { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    public class MailTmAttachment
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("disposition")]
        public string Disposition { get; set; }

        [JsonProperty("transferEncoding")]
        public string TransferEncoding { get; set; }

        [JsonProperty("related")]
        public bool Related { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }
    }

    public class MailTmAddress
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class MailTmToken
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}
