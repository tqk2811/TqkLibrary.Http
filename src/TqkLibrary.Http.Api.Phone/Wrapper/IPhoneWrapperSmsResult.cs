using System.Collections.Generic;

namespace TqkLibrary.Http.Api.Phone.Wrapper
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPhoneWrapperSmsResult<out T> : IEnumerable<T> where T : IPhoneWrapperSms
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsTimeout { get; }
    }
}
