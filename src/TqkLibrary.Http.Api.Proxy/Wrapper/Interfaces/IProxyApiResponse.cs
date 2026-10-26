using System;

namespace TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProxyApiResponseWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// 
        /// </summary>
        IProxyInfo? Proxy { get; }

        /// <summary>
        /// Next time request
        /// </summary>
        DateTime? NextTime { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime? ExpiredTime { get; }

        /// <summary>
        /// 
        /// </summary>
        string? Message { get; }
    }
}
