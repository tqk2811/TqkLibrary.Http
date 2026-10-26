using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace TqkLibrary.Http.Api.Phone
{
    /// <summary>
    /// https://docs.5sim.net/
    /// </summary>
    public class FiveSimNetApi : BaseApi
    {
        const string EndPoint = "https://5sim.net/v1";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public FiveSimNetApi(string apiKey) : base(apiKey)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<FiveSimNetBalance> GetBalance(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "user", "profile"))
            .ExecuteAsync<FiveSimNetBalance>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Dictionary<string, FiveSimNetProduct>> Products(
            string country = "any",
            string @operator = "any",
            CancellationToken cancellationToken = default)
           => Build()
           .WithUrlGet(new UrlBuilder(EndPoint, "guest", "products", country, @operator))
           .ExecuteAsync<Dictionary<string, FiveSimNetProduct>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Dictionary<string, Dictionary<string, FiveSimNetPrice>>> Prices(CancellationToken cancellationToken = default)
            => Build()
           .WithUrlGet(new UrlBuilder(EndPoint, "guest", "prices"))
           .ExecuteAsync<Dictionary<string, Dictionary<string, FiveSimNetPrice>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Dictionary<string, Dictionary<string, FiveSimNetPrice>>> PricesByCountry(string country, CancellationToken cancellationToken = default)
            => Build()
           .WithUrlGet(new UrlBuilder(EndPoint, "guest", "prices").WithParam("country", country))
           .ExecuteAsync<Dictionary<string, Dictionary<string, FiveSimNetPrice>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Dictionary<string, Dictionary<string, FiveSimNetPrice>>> PricesByProduct(string product, CancellationToken cancellationToken = default)
            => Build()
           .WithUrlGet(new UrlBuilder(EndPoint, "guest", "prices").WithParam("product", product))
           .ExecuteAsync<Dictionary<string, Dictionary<string, FiveSimNetPrice>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<Dictionary<string, Dictionary<string, FiveSimNetPrice>>> PricesByProduct(string country, string product, CancellationToken cancellationToken = default)
            => Build()
           .WithUrlGet(new UrlBuilder(EndPoint, "guest", "prices").WithParam("country", country).WithParam("product", product))
           .ExecuteAsync<Dictionary<string, Dictionary<string, FiveSimNetPrice>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<FiveSimNetNumber> BuyActivationNumber(string country, string @operator, string product, CancellationToken cancellationToken = default)
            => Build()
           .WithUrlGet(new UrlBuilder(EndPoint, "user/buy/activation", country, @operator, product))
           .ExecuteAsync<FiveSimNetNumber>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<FiveSimNetNumber> BuyHostingNumber(string country, string @operator, string product, CancellationToken cancellationToken = default)
            => Build()
           .WithUrlGet(new UrlBuilder(EndPoint, "user/buy/hosting", country, @operator, product))
           .ExecuteAsync<FiveSimNetNumber>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="product"></param>
        /// <param name="number">Phone number, 4-15 digits (without the + sign)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<string> ReBuyNumber(string product, string number, CancellationToken cancellationToken = default)
            => Build()
           .WithUrlGet(new UrlBuilder(EndPoint, "user/reuse", product, number))
           .ExecuteAsync<string>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<FiveSimNetNumber> CheckOrder(FiveSimNetNumber order, CancellationToken cancellationToken = default)
            => Build()
           .WithUrlGet(new UrlBuilder(EndPoint, "user/check", order.Id.ToString()))
           .ExecuteAsync<FiveSimNetNumber>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<FiveSimNetNumber> FinishOrder(FiveSimNetNumber order, CancellationToken cancellationToken = default)
           => Build()
          .WithUrlGet(new UrlBuilder(EndPoint, "user/finish", order.Id.ToString()))
          .ExecuteAsync<FiveSimNetNumber>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<FiveSimNetNumber> CancelOrder(FiveSimNetNumber order, CancellationToken cancellationToken = default)
           => Build()
          .WithUrlGet(new UrlBuilder(EndPoint, "user/cancel", order.Id.ToString()))
          .ExecuteAsync<FiveSimNetNumber>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<FiveSimNetNumber> BanOrder(FiveSimNetNumber order, CancellationToken cancellationToken = default)
           => Build()
          .WithUrlGet(new UrlBuilder(EndPoint, "user/ban", order.Id.ToString()))
          .ExecuteAsync<FiveSimNetNumber>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<FiveSimNetList<FiveSimNetSms>> SmsInboxList(FiveSimNetNumber order, CancellationToken cancellationToken = default)
           => Build()
          .WithUrlGet(new UrlBuilder(EndPoint, "user/sms/inbox", order.Id.ToString()))
          .ExecuteAsync<FiveSimNetList<FiveSimNetSms>>(cancellationToken);
    }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FiveSimNetList<T>
    {
        public List<T> Data { get; set; }
        public int Total { get; set; }
    }
    public class FiveSimNetNumber
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("operator")]
        public string Operator { get; set; }

        [JsonProperty("product")]
        public string Product { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("status")]
        public FiveSimNetOrderStatuses Status { get; set; }

        [JsonProperty("expires")]
        public string Expires { get; set; }

        [JsonProperty("sms")]
        public List<FiveSimNetSms> Sms { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("forwarding")]
        public bool Forwarding { get; set; }

        [JsonProperty("forwarding_number")]
        public string ForwardingNumber { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }
    public class FiveSimNetSms
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public enum FiveSimNetOrderStatuses
    {
        PENDING,
        RECEIVED,
        CANCELED,
        TIMEOUT,
        FINISHED,
        BANNED
    }
    public class FiveSimNetPrice
    {
        [JsonProperty("cost")]
        public double Cost { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
    public class FiveSimNetProduct
    {
        public string Category { get; set; }
        public int Qty { get; set; }
        public int Price { get; set; }
    }

    public class FiveSimNetBalance
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("vendor")]
        public string Vendor { get; set; }

        [JsonProperty("default_forwarding_number")]
        public string DefaultForwardingNumber { get; set; }

        [JsonProperty("balance")]
        public double Balance { get; set; }

        [JsonProperty("rating")]
        public double Rating { get; set; }

        [JsonProperty("default_country")]
        public FiveSimNetCountry DefaultCountry { get; set; }

        [JsonProperty("default_operator")]
        public FiveSimNetOperator DefaultOperator { get; set; }

        [JsonProperty("frozen_balance")]
        public double FrozenBalance { get; set; }
    }

    public class FiveSimNetCountry
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("iso")]
        public string Iso { get; set; }

        [JsonProperty("prefix")]
        public string Prefix { get; set; }
    }

    public class FiveSimNetOperator
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
