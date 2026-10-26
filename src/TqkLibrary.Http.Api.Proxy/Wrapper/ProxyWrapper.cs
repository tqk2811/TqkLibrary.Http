using System;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces;
using TqkLibrary.Http.Api.Proxy.Wrapper.Enums;

namespace TqkLibrary.Http.Api.Proxy.Wrapper
{
    internal class ProxyWrapper : IProxyWrapper
    {
        readonly ProxyApiItemData _proxyApiItemData;
        internal ProxyWrapper(ProxyApiItemData proxyApiItemData)
        {
            this._proxyApiItemData = proxyApiItemData ?? throw new ArgumentNullException(nameof(proxyApiItemData));
            proxyApiItemData.AddRef();
        }
        ~ProxyWrapper()
        {
            _proxyApiItemData.RemoveRef();
        }

        public IProxyInfo? ProxyInfo => _proxyApiItemData.CurrentProxy;



#if NET5_0_OR_GREATER
        public async ValueTask DisposeAsync()
        {
            await _proxyApiItemData.RemoveRefAsync();
            GC.SuppressFinalize(this);
        }
#endif
        public void Dispose()
        {
            _proxyApiItemData.RemoveRef();
            GC.SuppressFinalize(this);
        }
    }
}
