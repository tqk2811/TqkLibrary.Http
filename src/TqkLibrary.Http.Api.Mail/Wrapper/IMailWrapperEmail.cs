using System;
namespace TqkLibrary.Http.Api.Mail.Wrapper
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMailWrapperEmail
    {
        /// <summary>
        /// 
        /// </summary>
        string? FromAddress { get; }
        /// <summary>
        /// 
        /// </summary>
        string? Subject { get; }
        /// <summary>
        /// 
        /// </summary>
        string? RawBody { get; }
        
        /// <summary>
        /// 
        /// </summary>
        string? Code { get; }

        /// <summary>
        /// 
        /// </summary>
        DateTime? ReceivedTime { get; }
    }
}
