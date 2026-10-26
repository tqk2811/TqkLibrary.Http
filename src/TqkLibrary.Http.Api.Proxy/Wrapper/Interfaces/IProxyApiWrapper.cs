using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IProxyApiWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<IProxyApiResponseWrapper?> GetNewProxyAsync(CancellationToken cancellationToken);
        /// <summary>
        /// true for allow get new proxy/reset api when some one are using it.<br>
        /// </br>Example: dcom proxy hub, reset will make current device using proxy disconnect. 
        /// </summary>
        public bool IsAllowGetNewOnUsing { get; }

    }
}
