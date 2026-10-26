using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces;

namespace TqkLibrary.Http.Api.Proxy.Wrapper
{
    /// <summary>
    /// 
    /// </summary>
    public class ProxyWrapperManaged
    {
        readonly Dictionary<IProxyApiWrapper, ProxyApiItemData> _dicts = new Dictionary<IProxyApiWrapper, ProxyApiItemData>();
        readonly AsyncLock _asyncLock = new AsyncLock();
        readonly ILogger? _logger;
        /// <summary>
        /// 
        /// </summary>
        public int Count => _dicts.Count;
        /// <summary>
        /// 
        /// </summary>
        public int MaxUseCountPerApi { get; set; } = int.MaxValue;
        /// <summary>
        /// 
        /// </summary>
        public int Delay { get; set; } = 100;

        /// <summary>
        /// Allow proxy life-time left greater than this<br>
        /// </br>set to TimeSpan.Zero is un-limit<br>
        /// </br>Default: TimeSpan.Zero
        /// </summary>
        public TimeSpan AllowTimeLeft { get; set; } = TimeSpan.Zero;


        public ProxyWrapperManaged()
        {
            this._logger = null;
        }
        public ProxyWrapperManaged(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public ProxyWrapperManaged(ILoggerFactory loggerFactory) : this(loggerFactory.CreateLogger<ProxyWrapperManaged>())
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxyApi"></param>
        /// <param name="isClear"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task LoadAsync(IProxyApiWrapper proxyApi, bool isClear = true)
        {
            using var l = await _asyncLock.LockAsync().ConfigureAwait(false);
            if (isClear) _dicts.Clear();
            _dicts[proxyApi] = new ProxyApiItemData();
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task LoadAsync(IEnumerable<IProxyApiWrapper> proxyApis, bool isClear = true)
        {
            var apis = proxyApis?.ToList() ?? throw new ArgumentNullException(nameof(proxyApis));
            if (apis.Any(x => x == null)) throw new ArgumentNullException($"Have item in {nameof(proxyApis)} null");
            using var l = await _asyncLock.LockAsync().ConfigureAwait(false);
            if (isClear) _dicts.Clear();
            foreach (var api in apis) _dicts[api] = new ProxyApiItemData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ClearAsync()
        {
            using var l = await _asyncLock.LockAsync().ConfigureAwait(false);
            _dicts.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IProxyWrapper?> GetProxyAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (await _asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (_dicts.Count == 0) return null;

                    var currTime = DateTime.Now;
                    //check is item can reset
                    var pair = _dicts.OrderBy(x => Guid.NewGuid()).FirstOrDefault(x =>
                        currTime > x.Value.NextReset &&
                        (x.Key.IsAllowGetNewOnUsing || x.Value.UsingCount == 0) &&
                        (AllowTimeLeft == TimeSpan.Zero || currTime + AllowTimeLeft < x.Value.ExpiredTime));
                    if (pair.Key != null)
                    {
                        _logger?.LogInformation("ProxyManaged.GetNewProxyAsync");
                        IProxyApiResponseWrapper? proxyApiResponse = await pair.Key.GetNewProxyAsync(cancellationToken).ConfigureAwait(false);
                        if (proxyApiResponse == null) throw new InvalidOperationException(nameof(proxyApiResponse));
                        pair.Value.Reset(proxyApiResponse);

                        if (proxyApiResponse.IsSuccess == true)
                        {
                            string log = $"ProxyManaged Got New Proxy {pair.Value.CurrentProxy} for key {pair.Key}";
                            _logger?.LogInformation(log);
                            return new ProxyWrapper(pair.Value);
                        }
                        else
                        {
                            string log = $"ProxyManaged key {pair.Key} wait change at {proxyApiResponse.NextTime:HH:mm:ss} {proxyApiResponse?.Message}";
                            _logger?.LogInformation(log);
                        }
                    }
                    else
                    {
                        //check is item allow using more times
                        pair = _dicts.FirstOrDefault(x => MaxUseCountPerApi > x.Value.UsedCount && x.Value.CurrentProxy is not null);
                        if (pair.Key != null)
                        {
                            string log = $"ProxyManaged key {pair.Key} re-use {pair.Value.CurrentProxy}";
                            _logger?.LogInformation(log);
                            return new ProxyWrapper(pair.Value);
                        }
                    }
                }

                await Task.Delay(Delay, cancellationToken);
            }
        }
    }
}
