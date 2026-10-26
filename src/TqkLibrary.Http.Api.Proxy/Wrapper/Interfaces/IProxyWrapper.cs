using System;
using TqkLibrary.Http.Api.Proxy.Wrapper.Enums;

namespace TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProxyWrapper : IDisposable
#if NET5_0_OR_GREATER
        , IAsyncDisposable
#endif
    {
        IProxyInfo? ProxyInfo { get; }
    }
}
