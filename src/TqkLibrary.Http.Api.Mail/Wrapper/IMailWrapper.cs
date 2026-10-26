using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Mail.Wrapper
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMailWrapper
    {
        /// <summary>
        /// -1 mean infinity
        /// </summary>
        int AccountAvailablesCount { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IMailWrapperAccount?> GetAccountAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task ReQueueAccountAsync(IMailWrapperAccount mailSession, CancellationToken cancellationToken = default);
    }
}
