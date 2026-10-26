using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Other.Others
{
    /// <summary>
    /// https://documenter.getpostman.com/view/9826758/TzzANcVu
    /// </summary>
    public class CloneMailSieuReApi : BaseApi
    {
        const string EndPoint = "https://www.clonemailsieure.site/api";
        readonly string userName;
        readonly string passWord;
        /// <summary>
        /// 
        /// </summary>
        public CloneMailSieuReApi(string userName, string passWord)
        {
            this.userName = userName;
            this.passWord = passWord;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CloneMailSieuReListResourceResponse> ListResource(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "ListResource.php")
                .WithParam("username", userName)
                .WithParam("password", passWord))
            .ExecuteAsync<CloneMailSieuReListResourceResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="amount"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CloneMailSieuReApiResponse<CloneMailSieuReResourceData>> BuyResource(CloneMailSieuReAccount account, int amount = 1, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "BResource.php")
                .WithParam("username", userName)
                .WithParam("password", passWord)
                .WithParam("id", account.Id)
                .WithParam("amount", amount))
            .ExecuteAsync<CloneMailSieuReApiResponse<CloneMailSieuReResourceData>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CloneMailSieuReApiResponse<CloneMailSieuReAccount>> InfoResource(CloneMailSieuReAccount account, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "InfoResource.php")
                .WithParam("username", userName)
                .WithParam("password", passWord)
                .WithParam("id", account.Id))
            .ExecuteAsync<CloneMailSieuReApiResponse<CloneMailSieuReAccount>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> GetBalance(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "GetBalance.php")
                .WithParam("username", userName)
                .WithParam("password", passWord))
            .ExecuteAsync<string>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ImportAccount(int product, string data, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "importAccount.php")
                .WithParam("username", userName)
                .WithParam("password", passWord)
                .WithParam("product", product)
                .WithParam("account", data))
            .ExecuteAsync<string>(cancellationToken);
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CloneMailSieuReAccount
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class CloneMailSieuReCategory
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("accounts")]
        public List<CloneMailSieuReAccount> Accounts { get; set; }
    }

    public class CloneMailSieuReListResourceResponse : CloneMailSieuReApiResponse
    {
        [JsonProperty("categories")]
        public List<CloneMailSieuReCategory> Categories { get; set; }
    }

    public class CloneMailSieuReApiResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }
    }
    public class CloneMailSieuReApiResponse<T> : CloneMailSieuReApiResponse
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }
    public class CloneMailSieuReResourceItem
    {
        [JsonProperty("account")]
        public string Account { get; set; }
    }

    public class CloneMailSieuReResourceData
    {
        [JsonProperty("trans_id")]
        public string TransId { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("time")]
        public int Time { get; set; }

        [JsonProperty("lists")]
        public List<CloneMailSieuReResourceItem> Lists { get; set; }
    }


#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
