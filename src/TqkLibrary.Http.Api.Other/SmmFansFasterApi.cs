using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Other
{
    /// <summary>
    /// 
    /// </summary>
    public class SmmFansFasterApi : BaseApi
    {
        const string EndPoint = "https://smmfansfaster.com/api/v2";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApiKey"></param>
        public SmmFansFasterApi(string ApiKey) : base(ApiKey)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<List<SmmFansFasterViewServiceResult>> ServiceList(CancellationToken cancellationToken = default)
        {
            var formPost = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("key", ApiKey),
                new KeyValuePair<string, string>("action", "services")
            });
            return Build()
                .WithUrlPost(EndPoint, formPost)
                .ExecuteAsync<List<SmmFansFasterViewServiceResult>>(cancellationToken);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<SmmFansFasterOrderResult> AddOrder(SmmFansFasterViewServiceResult smmFansFasterViewServiceResult, string link, int quantity, CancellationToken cancellationToken = default)
        {
            var formPost = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("key", ApiKey),
                new KeyValuePair<string, string>("action", "add"),
                new KeyValuePair<string, string>("service", smmFansFasterViewServiceResult.service.ToString()),
                new KeyValuePair<string, string>("link", link),
                new KeyValuePair<string, string>("quantity", quantity.ToString())
            });
            return Build()
                .WithUrlPost(EndPoint, formPost)
                .ExecuteAsync<SmmFansFasterOrderResult>(cancellationToken);
        }
    }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class SmmFansFasterViewServiceResult
    {
        public int service { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string category { get; set; }
        public double rate { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public bool dripfeed { get; set; }
        public bool refill { get; set; }

        public override string ToString()
        {
            return $"name: {name}, type: {type}, category: {category}";
        }
    }
    public class SmmFansFasterOrderResult
    {
        public int order { get; set; }
        public string error { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
