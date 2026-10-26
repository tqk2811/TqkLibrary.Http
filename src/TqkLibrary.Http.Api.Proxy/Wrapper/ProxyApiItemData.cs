using System;
using System.Threading.Tasks;
using Nito.AsyncEx;
using TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces;
using TqkLibrary.Http.Api.Proxy.Wrapper.Enums;

namespace TqkLibrary.Http.Api.Proxy.Wrapper
{
    internal class ProxyApiItemData
    {
        readonly AsyncLock _asyncLock = new AsyncLock();
        public IProxyInfo? CurrentProxy { get; private set; }
        public DateTime NextReset { get; set; } = DateTime.Now;
        public DateTime? ExpiredTime { get; private set; }
        public int UsedCount { get; private set; } = 0;
        public int UsingCount { get; private set; } = 0;
        public void Reset(IProxyApiResponseWrapper proxyApiResponse)
        {
            if (proxyApiResponse == null) throw new ArgumentNullException(nameof(proxyApiResponse));
            NextReset = proxyApiResponse.NextTime.HasValue ? proxyApiResponse.NextTime.Value : DateTime.Now;
            if (proxyApiResponse.IsSuccess)
            {
                UsedCount = 0;
                UsingCount = 0;
                CurrentProxy = proxyApiResponse.Proxy!;
                ExpiredTime = proxyApiResponse.ExpiredTime;
            }
        }
        public void AddRef()
        {
            using var l = _asyncLock.Lock();
            UsingCount++;
            UsedCount++;
        }
        public async Task AddRefAsync()
        {
            using var l = await _asyncLock.LockAsync().ConfigureAwait(false);
            UsingCount++;
            UsedCount++;
        }
        public void RemoveRef()
        {
            using var l = _asyncLock.Lock();
            UsingCount--;
        }
        public async Task RemoveRefAsync()
        {
            using var l = await _asyncLock.LockAsync().ConfigureAwait(false);
            UsingCount--;
        }
    }
}
