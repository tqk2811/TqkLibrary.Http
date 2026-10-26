using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Proxy.Services;
using TqkLibrary.Http.Api.Proxy.Wrapper;
using TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces;
using TqkLibrary.Http.Api.Proxy.Wrapper.Enums;

namespace TqkLibrary.Http.Api.Proxy.Wrapper.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class TinProxyApiWrapper : IProxyApiWrapper
    {
        readonly TinProxyApi tinProxyApi;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public TinProxyApiWrapper(string apiKey)
        {
            this.tinProxyApi = new TinProxyApi(apiKey);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tinProxyApi"></param>
        public TinProxyApiWrapper(TinProxyApi tinProxyApi)
        {
            this.tinProxyApi = tinProxyApi;
        }


        /// <summary>
        /// 
        /// </summary>
        public bool IsAllowGetNewOnUsing => true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IProxyApiResponseWrapper?> GetNewProxyAsync(CancellationToken cancellationToken)
        {
            var res = await tinProxyApi.GetNewProxy(cancellationToken).ConfigureAwait(false);

            ProxyApiResponseWrapper result = new ProxyApiResponseWrapper()
            {
                IsSuccess = res.Code == 1,
                NextTime = DateTime.Now.AddSeconds(res.Data.NextRequest),
                ExpiredTime = DateTime.Now.AddSeconds(res.Data.Timeout),
                Message = res.Message,
            };
            if (result.IsSuccess)
            {
                result.Proxy = ProxyInfo.ParseHttpProxy(res.Data.HttpIpv4);
                result.IsSuccess = result.Proxy is not null;
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string? ToString()
        {
            return tinProxyApi.ToString();
        }
    }
}
