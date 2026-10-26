using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Phone
{
    /// <summary>
    /// 
    /// </summary>
    public class KfarmTextNowApi : BaseApi
    {
        const string EndPoint = "http://kfarm.vn/api/TextNow/";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        public KfarmTextNowApi(string token) : base(token)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<KfarmTextNowResponse<KfarmTextNowPhone>> GetAccTextNow(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(EndPoint + "GetAccTextNow")
            .WithHeader("Token", ApiKey)
            .ExecuteAsync<KfarmTextNowResponse<KfarmTextNowPhone>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<KfarmTextNowResponse<KfarmTextNowOrder>> GetOrderTextNow(KfarmTextNowPhone phone, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlPostJson(EndPoint + "GetOrderTextNow", phone)
            .WithHeader("Token", ApiKey)
            .ExecuteAsync<KfarmTextNowResponse<KfarmTextNowOrder>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<KfarmTextNowResponse<KfarmTextNowCode>> GetCode(KfarmTextNowOrder orderId, CancellationToken cancellationToken = default)
             => Build()
            .WithUrlPostJson(EndPoint + "GetOrderTextNow", orderId)
            .WithHeader("Token", ApiKey)
            .ExecuteAsync<KfarmTextNowResponse<KfarmTextNowCode>>(cancellationToken);
    }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class KfarmTextNowResponse<T>
    {
        public int status_code { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }
    public class KfarmTextNowPhone
    {
        public string phone { get; set; }
    }
    public class KfarmTextNowOrder
    {
        public string order_id { get; set; }
    }
    public class KfarmTextNowCode
    {
        public string code { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
