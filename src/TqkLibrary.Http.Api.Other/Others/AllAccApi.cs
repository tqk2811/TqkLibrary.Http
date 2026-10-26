using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Other.Others
{
    /// <summary>
    /// 
    /// </summary>
    public class AllAccApi : BaseApi
    {
        const string EndPoint = "http://allacc.vn/api/";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public AllAccApi(string apiKey) : base(apiKey)
        {

        }

        class ApiPost
        {
            public string api_key { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AllAccApiBalanceResponse> Balance(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlPostJson(new UrlBuilder(EndPoint, "v1/balance"), new ApiPost() { api_key = ApiKey })
            .ExecuteAsync<AllAccApiBalanceResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AllAccData<AllAccCategory>> Categories(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "v1/categories"))
            .ExecuteAsync<AllAccData<AllAccCategory>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AllAccData<AllAccProduct>> Product(AllAccCategory category, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint, "v1/category", category.Id.ToString()))
            .ExecuteAsync<AllAccData<AllAccProduct>>(cancellationToken);

        class BuyPost : ApiPost
        {
            public int id_product { get; set; }
            public int quantity { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AllAccApiBuyResponse> Buy(AllAccProduct product, int buyCount = 1, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlPostJson(new UrlBuilder(EndPoint, "v1/buy"), new BuyPost() { api_key = ApiKey, id_product = product.Id, quantity = buyCount })
            .ExecuteAsync<AllAccApiBuyResponse>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AllAccData<AllAccOrderItem>> Orders(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlPostJson(new UrlBuilder(EndPoint, "v1/orders"), new ApiPost() { api_key = ApiKey })
            .ExecuteAsync<AllAccData<AllAccOrderItem>>(cancellationToken);

        class OrderPost : ApiPost
        {
            public long order_id { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AllAccOrderResponse<T>> Order<T>(AllAccOrderItem order, CancellationToken cancellationToken = default) where T : class, IAllAccOrderItem
            => Build()
            .WithUrlPostJson(new UrlBuilder(EndPoint, "v1/order"), new OrderPost() { api_key = ApiKey, order_id = order.Id })
            .ExecuteAsync<AllAccOrderResponse<T>>(cancellationToken);
    }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class AllAccApiResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; }
    }

    public class AllAccApiBalanceResponse : AllAccApiResponse
    {
        [JsonProperty("balance")]
        public long Balance { get; set; }
    }
    public class AllAccApiBuyResponse : AllAccApiResponse
    {
        [JsonProperty("order_id")]
        public long OrderId { get; set; }
    }
    public class AllAccOrderItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("total_price")]
        public long TotalPrice { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }


        public static implicit operator AllAccOrderItem(int id)
        {
            return new AllAccOrderItem { Id = id };
        }

    }
    public class AllAccOrderResponse<T> : AllAccData<T>
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("total_price")]
        public long TotalPrice { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
    public class AllAccData<T>
    {
        [JsonProperty("data")]
        public List<T> Categories { get; set; }
    }
    public class AllAccCategory
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("list_product")]
        public List<AllAccProduct> Products { get; set; }

        public static implicit operator AllAccCategory(int id)
        {
            return new AllAccCategory { Id = id };
        }
    }
    public class AllAccProduct
    {
        [JsonProperty("id_product")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("desc")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        public static implicit operator AllAccProduct(int id)
        {
            return new AllAccProduct { Id = id };
        }
    }
    public interface IAllAccOrderItem
    {

    }
    public class AllAccOrderItemFacebook : IAllAccOrderItem
    {
        [JsonProperty("uid")]
        public int Uid { get; set; }

        [JsonProperty("full_info")]
        public string FullInfo { get; set; }

        [JsonProperty("link_backup")]
        public string LinkBackup { get; set; }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
