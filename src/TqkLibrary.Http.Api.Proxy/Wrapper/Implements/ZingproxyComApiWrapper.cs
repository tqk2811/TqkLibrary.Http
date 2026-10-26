using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Proxy.Services;
using TqkLibrary.Http.Api.Proxy.Wrapper;
using TqkLibrary.Http.Api.Proxy.Wrapper.Enums;
using TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces;

namespace TqkLibrary.Http.Api.Proxy.Wrapper.Implements
{
    public class ZingproxyComApiWrapper : IProxyApiWrapper
    {
        /// <summary>
        /// socks5 or http proxy
        /// </summary>
        public ProxyType ProxyType { get; set; } = ProxyType.Http;
        public bool IsAllowGetNewOnUsing => false;


        readonly ZingproxyComApi _zingproxyComApi;
        readonly ZingproxyComApi.ProxyFullInfo _proxyFullInfo;
        public ZingproxyComApiWrapper(ZingproxyComApi zingproxyComApi, ZingproxyComApi.ProxyFullInfo proxyFullInfo)
        {
            this._zingproxyComApi = zingproxyComApi ?? throw new ArgumentNullException(nameof(zingproxyComApi));
            this._proxyFullInfo = proxyFullInfo ?? throw new ArgumentNullException(nameof(proxyFullInfo));
        }




        ZingproxyComApi.ProxyFullInfo? _LastProxyFullInfo = null;
        int? allowChangeInSecond = null;
        static readonly Regex regex = new Regex("\\d+");
        public virtual async Task<IProxyApiResponseWrapper?> GetNewProxyAsync(CancellationToken cancellationToken)
        {
            ZingproxyComApi.ProxyFullInfo? proxyFullInfo = null;
            string? message = null;
            try
            {
                var res = await _zingproxyComApi.DanCuVietNam.GetIpAsync(_proxyFullInfo, cancellationToken: cancellationToken);
                proxyFullInfo = res.Proxy;
                _LastProxyFullInfo = res.Proxy;
                message = res.Message ?? res.Error;
                allowChangeInSecond = null;
            }
            catch (ApiException<ZingproxyComApi.BaseResponse> apiex)//use old
            {
                Match match = regex.Match(apiex.Body?.Error ?? string.Empty);
                if (match.Success)
                {
                    allowChangeInSecond = int.Parse(match.Value);
                }
            }
            if (proxyFullInfo is null)
            {
                proxyFullInfo = _LastProxyFullInfo ?? _proxyFullInfo;
            }

            ProxyApiResponseWrapper result = new ProxyApiResponseWrapper()
            {
                IsSuccess = proxyFullInfo is not null,
            };
            if (result.IsSuccess)
            {
                result.NextTime = proxyFullInfo.ObjectCreateTime.AddSeconds(allowChangeInSecond ?? proxyFullInfo.TimeChangeAllowInSeconds ?? 240);
                result.ExpiredTime = proxyFullInfo.DateEnd;
                if (ProxyType.HasFlag(ProxyType.Http) && (proxyFullInfo?.PortHttp).HasValue)
                {
                    result.Proxy = new ProxyInfo()
                    {
                        Address = proxyFullInfo.Ip,
                        Port = proxyFullInfo.PortHttp.Value,
                        ProxyType = ProxyType.Http,
                        UserName = proxyFullInfo.Username,
                        Password = proxyFullInfo.Password,
                    };
                }
                else if (ProxyType.HasFlag(ProxyType.Socks5) && (proxyFullInfo?.PortSocks5).HasValue)
                {
                    result.Proxy = new ProxyInfo()
                    {
                        Address = proxyFullInfo.Ip,
                        Port = proxyFullInfo.PortSocks5.Value,
                        ProxyType = ProxyType.Socks5,
                        UserName = proxyFullInfo.Username,
                        Password = proxyFullInfo.Password,
                    };
                }
                else if ((proxyFullInfo?.PortHttp).HasValue)
                {
                    result.Proxy = new ProxyInfo()
                    {
                        Address = proxyFullInfo.Ip,
                        Port = proxyFullInfo.PortHttp.Value,
                        ProxyType = ProxyType.Http,
                        UserName = proxyFullInfo.Username,
                        Password = proxyFullInfo.Password,
                    };
                }
                else if ((proxyFullInfo?.PortSocks5).HasValue)
                {
                    result.Proxy = new ProxyInfo()
                    {
                        Address = proxyFullInfo.Ip,
                        Port = proxyFullInfo.PortHttp.Value,
                        ProxyType = ProxyType.Http,
                        UserName = proxyFullInfo.Username,
                        Password = proxyFullInfo.Password,
                    };
                }

                result.IsSuccess = result.Proxy is not null;
            }
            return result;
        }
    }
}
