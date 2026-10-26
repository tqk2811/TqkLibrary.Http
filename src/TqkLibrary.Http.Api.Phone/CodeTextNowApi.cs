using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Phone
{
    /// <summary>
    /// 
    /// </summary>
    public class CodeTextNowApi : BaseApi
    {
        const string EndPoint = "http://codetextnow.com/api.php";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public CodeTextNowApi(string apiKey) : base(apiKey)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<List<CodeTextNowService>> Services(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint)
                .WithParam("action", "services")
                .WithParam("apikey", ApiKey))
            .ExecuteAsync<List<CodeTextNowService>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<CodeTextNowCreateRequest> CreateRequest(CodeTextNowService service, int count = 1, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint)
                .WithParam("action", "create-request")
                .WithParam("apikey", ApiKey)
                .WithParam("serviceId", service.serviceId)
                .WithParam("count", count))
            .ExecuteAsync<CodeTextNowCreateRequest>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<CodeTextNowDataRequest> DataRequest(CodeTextNowRent rent, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint)
                .WithParam("action", "data-request")
                .WithParam("apikey", ApiKey)
                .WithParam("rentId", rent.requestId))
            .ExecuteAsync<CodeTextNowDataRequest>(cancellationToken);

    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CodeTextNowCreateRequest : CodeTextNowResponse<CodeTextNowResultResponse<List<CodeTextNowRent>>> { }
    public class CodeTextNowDataRequest : CodeTextNowResponse2<List<CodeTextNowData>> { }





    public class CodeTextNowService
    {
        public string name { get; set; }
        public int serviceId { get; set; }
        public int price { get; set; }
        public int timeout { get; set; }
    }



    public class CodeTextNowResponse<T>
    {
        public int status { get; set; }
        public T results { get; set; }
        public string message { get; set; }
    }


    public class CodeTextNowResultResponse<T>
    {
        public T data { get; set; }
    }

    public class CodeTextNowRent
    {
        public string name { get; set; }
        public string sdt { get; set; }
        public string otp { get; set; }
        public int status { get; set; }
        public long created_time { get; set; }
        public int requestId { get; set; }
    }






    public class CodeTextNowResponse2<T>
    {
        public string message { get; set; }
        public int status { get; set; }
        public T data { get; set; }
        public int? recordsFiltered { get; set; }
        public int? recordsTotal { get; set; }
    }
    public class CodeTextNowData
    {
        public int id { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public string sdt { get; set; }
        public string otp { get; set; }
        public string textSMS { get; set; }
        public long finishtime { get; set; }
        public int status { get; set; }
        public string created_time { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
