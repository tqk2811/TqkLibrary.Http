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
    public class AzProxyApiWrapper : IProxyApiWrapper
    {
        readonly AzProxyApi azProxyApi;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="azProxyApi"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AzProxyApiWrapper(AzProxyApi azProxyApi)
        {
            this.azProxyApi = azProxyApi ?? throw new ArgumentNullException(nameof(azProxyApi));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public AzProxyApiWrapper(string apiKey)
        {
            this.azProxyApi = new AzProxyApi(apiKey);
        }
        /// <summary>
        /// 
        /// </summary>
        public AzProxyApi.AzProxyLocation? Location { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public AzProxyApi.AzProxyProvider? Provider { get; set; }


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
            var proxy = await azProxyApi.GetNewProxy(Location, Provider, cancellationToken).ConfigureAwait(false)!;
            var proxy2 = await azProxyApi.GetNewProxy(Location, Provider, cancellationToken).ConfigureAwait(false)!;
            DateTime nextTime = DateTime.Now.Add(proxy2.NextTime.HasValue ? proxy2.NextTime.Value : TimeSpan.FromSeconds(10));
            ProxyApiResponseWrapper proxyApiResponseWrapper = new ProxyApiResponseWrapper()
            {
                IsSuccess = proxy.IsSuccess,
                NextTime = nextTime,
                ExpiredTime = nextTime.AddMinutes(20),
                Message = proxy?.Message,
            };
            if (proxy!.IsSuccess && !string.IsNullOrWhiteSpace(proxy!.Data?.Proxy))
            {
                ProxyInfo? proxyInfo = ProxyInfo.ParseHttpProxy(proxy!.Data?.Proxy);
                proxyApiResponseWrapper.Proxy = proxyInfo;
                proxyApiResponseWrapper.IsSuccess = proxyInfo != null;
            }
            return proxyApiResponseWrapper;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"AzProxy ({azProxyApi.ApiKey})";
        }
    }
}
