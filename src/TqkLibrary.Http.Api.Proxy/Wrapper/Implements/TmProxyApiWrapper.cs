using System;
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
    public class TmProxyApiWrapper : IProxyApiWrapper
    {
        readonly TmProxyApi tmProxyApi;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tmProxyApi"></param>
        public TmProxyApiWrapper(TmProxyApi tmProxyApi)
        {
            this.tmProxyApi = tmProxyApi ?? throw new ArgumentNullException(nameof(tmProxyApi));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public TmProxyApiWrapper(string apiKey)
        {
            this.tmProxyApi = new TmProxyApi(apiKey);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Location { get; set; } = 0;
        /// <summary>
        /// 
        /// </summary>
        public bool IsAllowGetNewOnUsing => true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IProxyApiResponseWrapper?> GetNewProxyAsync(CancellationToken cancellationToken)
        {
            var result = await tmProxyApi.GetNewProxy(Location).ConfigureAwait(false);
            ProxyApiResponseWrapper responseWrapper = new ProxyApiResponseWrapper()
            {
                IsSuccess = result.code == 0,
                NextTime = DateTime.Now.AddSeconds(result?.data.next_request ?? 5),
                ExpiredTime = result?.data.ExpiredAt ?? DateTime.Now,
                Message = result?.message,
            };
            if (responseWrapper.IsSuccess)
            {
                responseWrapper.Proxy = ProxyInfo.ParseHttpProxy(result!.data.https);
                responseWrapper.IsSuccess = responseWrapper.Proxy is not null;
            }

            return responseWrapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"TmProxy ({tmProxyApi.ApiKey})";
        }
    }
}
