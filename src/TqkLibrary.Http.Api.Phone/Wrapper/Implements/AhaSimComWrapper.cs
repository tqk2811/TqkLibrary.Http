using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Phone.Wrapper;

namespace TqkLibrary.Http.Api.Phone.Wrapper.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class AhaSimComWrapper : IPhoneWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        public AhaSimComService AhaSimComService { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<AhaSimComNetwork> Networks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Prefixs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> ExceptPrefixs { get; set; }


        readonly AhaSimComApi ahaSimComApi;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ahaSimComApi"></param>
        public AhaSimComWrapper(AhaSimComApi ahaSimComApi)
        {
            this.ahaSimComApi = ahaSimComApi;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public AhaSimComWrapper(string apiKey)
        {
            this.ahaSimComApi = new AhaSimComApi(apiKey);
        }
        /// <summary>
        /// 
        /// </summary>
        ~AhaSimComWrapper()
        {
            ahaSimComApi.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            ahaSimComApi.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IPhoneWrapperSession> RentPhoneAsync(CancellationToken cancellationToken = default)
        {
            if (AhaSimComService == null) throw new InvalidOperationException($"Set value to {nameof(AhaSimComService)} first");
            var session = await ahaSimComApi.PhoneNewSession(AhaSimComService, Networks, Prefixs, ExceptPrefixs, cancellationToken).ConfigureAwait(false);
            return new AhaSimComWrapperSession(session, ahaSimComApi);
        }
    }

    internal class AhaSimComWrapperSession : IPhoneWrapperSession
    {
        readonly AhaSimComResponse<AhaSimComSession> ahaSimComSession;
        readonly AhaSimComApi ahaSimComApi;
        internal AhaSimComWrapperSession(AhaSimComResponse<AhaSimComSession> ahaSimComSession, AhaSimComApi ahaSimComApi)
        {
            this.ahaSimComSession = ahaSimComSession;
            this.ahaSimComApi = ahaSimComApi;
        }

        public string PhoneNumber => ahaSimComSession?.Data?.PhoneNumber;

        public bool IsSuccess => ahaSimComSession.Success;

        public string Message => ahaSimComSession.Message;

        public Task CancelWaitSmsAsync(CancellationToken cancellationToken = default)
        {
            return ahaSimComApi.SessionCancel(ahaSimComSession.Data, cancellationToken);
        }

        public async Task<IPhoneWrapperSmsResult<IPhoneWrapperSms>> GetSmsAsync(CancellationToken cancellationToken = default)
        {
            var messages = await ahaSimComApi.SessionGetOtp(ahaSimComSession.Data, cancellationToken).ConfigureAwait(false);
            return new AhaSimComWrapperSmsResult(false, new AhaSimComWrapperSms(messages.Data));
        }
    }

    internal class AhaSimComWrapperSmsResult : List<AhaSimComWrapperSms>, IPhoneWrapperSmsResult<AhaSimComWrapperSms>
    {
        public AhaSimComWrapperSmsResult(bool isTimeout, AhaSimComWrapperSms wrapperSms)
        {
            this.IsTimeout = isTimeout;
            this.Add(wrapperSms);
        }
        public bool IsTimeout { get; }
    }

    internal class AhaSimComWrapperSms : IPhoneWrapperSms
    {
        readonly AhaSimComOtp ahaSimComOtp;
        internal AhaSimComWrapperSms(AhaSimComOtp ahaSimComOtp)
        {
            this.ahaSimComOtp = ahaSimComOtp;
        }
        public string Text => ahaSimComOtp?.Message?.Content;

        public string Code => ahaSimComOtp?.Message?.Otp;
    }
}
