using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Mail.Services.BuyMailApi
{
    /// <summary>
    /// 
    /// </summary>
    public class DongVanFbApi : BaseApi
    {
        const string EndPoint = "https://api.dongvanfb.com/";
        const string EndPointTools = " https://tools.dongvanfb.com/api/";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public DongVanFbApi(string apiKey) : base(apiKey)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<DongVanFbBalanceResponse> CheckBalance(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "user", "balance").WithParam("apikey", ApiKey))
            .ExecuteAsync<DongVanFbBalanceResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<DongVanFbAccountTypeResponse> AccountType(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "user", "account_type").WithParam("apikey", ApiKey))
            .ExecuteAsync<DongVanFbAccountTypeResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<DongVanFbBuyMailResponse> BuyMail(DongVanFbAccountType accountType, int quality = 1, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "user", "buy")
                .WithParam("apikey", ApiKey)
                .WithParam("account_type", accountType.Id)
                .WithParam("quality", quality))
            .ExecuteAsync<DongVanFbBuyMailResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<DongVanFbGetCodeMailResponse> GetCodeMail(DongVanFbMailAccount mailAccount, MailFrom? mailFrom = null, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPointTools, "get_code")
                .WithParam("mail", mailAccount.Email)
                .WithParam("pass", mailAccount.Password)
                .WithParamIfNotNull("type", mailFrom?.ToString()?.ToLower()))
            .ExecuteAsync<DongVanFbGetCodeMailResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<DongVanFbGetMessageResponse> GetMessages(DongVanFbMailAccount mailAccount, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPointTools, "get_messages")
                .WithParam("mail", mailAccount.Email)
                .WithParam("pass", mailAccount.Password))
            .ExecuteAsync<DongVanFbGetMessageResponse>(cancellationToken);







#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public class DongVanFbResponse
        {
            [JsonProperty("error_code")]
            public int ErrorCode { get; set; }

            [JsonProperty("status")]
            public bool Status { get; set; }

            [JsonProperty("message")]
            public string? Message { get; set; }
        }
        public class DongVanFbResponse<T> : DongVanFbResponse
        {
            [JsonProperty("data")]
            public T? Data { get; set; }
        }
        public class DongVanFbBalanceResponse : DongVanFbResponse
        {
            [JsonProperty("balance")]
            public double Balance { get; set; }
        }
        public class DongVanFbAccountType
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("quality")]
            public int Quality { get; set; }

            [JsonProperty("price")]
            public double Price { get; set; }
        }
        public class DongVanFbAccountTypeResponse : DongVanFbResponse<List<DongVanFbAccountType>>
        {

        }
        public class DongVanFbBuyMail
        {
            [JsonProperty("order_code")]
            public string? OrderCode { get; set; }

            [JsonProperty("account_type")]
            public string? AccountType { get; set; }

            [JsonProperty("quality")]
            public int Quality { get; set; }

            [JsonProperty("price")]
            public int Price { get; set; }

            [JsonProperty("total_amount")]
            public int TotalAmount { get; set; }

            [JsonProperty("balance")]
            public int Balance { get; set; }

            [JsonProperty("list_data")]
            public List<string>? ListData { get; set; }

            [JsonIgnore]
            public IEnumerable<DongVanFbMailAccount> ListDataAccount
            {
                get { return ListData?.Select(x => new DongVanFbMailAccount(x)) ?? Enumerable.Empty<DongVanFbMailAccount>(); }
            }

        }
        public class DongVanFbBuyMailResponse : DongVanFbResponse<DongVanFbBuyMail>
        {

        }
        public class DongVanFbMailAccount
        {
            public DongVanFbMailAccount(string text)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var arr = text.Split('|');
                    if (arr.Length >= 2)
                    {
                        this.Email = arr[0];
                        this.Password = arr[1];
                    }
                }
            }
            public string? Email { get; set; }
            public string? Password { get; set; }
        }



        public class DongVanFbToolResponse
        {
            [JsonProperty("email")]
            public string? Email { get; set; }

            [JsonProperty("password")]
            public string? Password { get; set; }

            [JsonProperty("status")]
            public bool Status { get; set; }

            [JsonProperty("code")]
            public string? Code { get; set; }
        }
        public class DongVanFbGetCodeMailResponse : DongVanFbToolResponse
        {

            [JsonProperty("content")]
            public string? Content { get; set; }

            [JsonProperty("date")]
            public string? Date { get; set; }
        }
        public enum MailFrom
        {
            Facebook,
            Instagram,
            Twitter,
        }


        public class DongVanFbGetMessageResponse : DongVanFbToolResponse
        {
            [JsonProperty("messages")]
            public List<DongVanFbMessage>? Messages { get; set; }
        }
        public class DongVanFbMessage
        {
            [JsonProperty("uid")]
            public int Uid { get; set; }

            [JsonProperty("date")]
            public DateTime Date { get; set; }

            [JsonProperty("from")]
            public List<DongVanFbMessageFrom>? From { get; set; }

            [JsonProperty("subject")]
            public string? Subject { get; set; }

            [JsonProperty("code")]
            public string? Code { get; set; }

            [JsonProperty("message")]
            public string? Message { get; set; }
        }
        public class DongVanFbMessageFrom
        {
            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("address")]
            public string? Address { get; set; }
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

}
