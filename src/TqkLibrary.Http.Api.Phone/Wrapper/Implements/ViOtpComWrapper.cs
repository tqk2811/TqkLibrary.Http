using System;
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
    public class ViOtpComWrapper : IPhoneWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        public ViOtpComService Service { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ViOtpComNetwork> Networks { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> Prefixs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<string> ExceptPrefixs { get; set; }

        readonly ViOtpComApi viOtpComApi;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viOtpComApi"></param>
        public ViOtpComWrapper(ViOtpComApi viOtpComApi)
        {
            this.viOtpComApi = viOtpComApi;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public ViOtpComWrapper(string apiKey)
        {
            this.viOtpComApi = new ViOtpComApi(apiKey);
        }
        /// <summary>
        /// 
        /// </summary>
        ~ViOtpComWrapper()
        {
            viOtpComApi.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            viOtpComApi.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IPhoneWrapperSession> RentPhoneAsync(CancellationToken cancellationToken = default)
        {
            if (Service == null) throw new InvalidOperationException($"{nameof(Service)} must be set");
            var res = await viOtpComApi.RequestRent(Service, Networks, Prefixs, ExceptPrefixs, cancellationToken);
            return new ViOtpComWrapperSession(viOtpComApi, res);
        }
    }

    internal class ViOtpComWrapperSession : IPhoneWrapperSession
    {
        readonly ViOtpComApi viOtpComApi;
        readonly ViOtpComResponse<ViOtpComSession> viOtpComSession;
        internal ViOtpComWrapperSession(ViOtpComApi viOtpComApi, ViOtpComResponse<ViOtpComSession> viOtpComSession)
        {
            this.viOtpComApi = viOtpComApi;
            this.viOtpComSession = viOtpComSession;
        }
        public string PhoneNumber
        {
            get
            {
                if (string.IsNullOrWhiteSpace(viOtpComSession?.Data?.PhoneNumber)) return string.Empty;
                if (viOtpComSession.Data.PhoneNumber.Length == 10 && viOtpComSession.Data.PhoneNumber.StartsWith("0")) return viOtpComSession.Data.PhoneNumber.Substring(1);
                return viOtpComSession.Data.PhoneNumber;
            }
        }

        public string Message => viOtpComSession?.Message;
        public bool IsSuccess => viOtpComSession?.Success == true;

        public Task CancelWaitSmsAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task<IPhoneWrapperSmsResult<IPhoneWrapperSms>> GetSmsAsync(CancellationToken cancellationToken = default)
        {
            var res = await viOtpComApi.SessionGet(viOtpComSession.Data).ConfigureAwait(false);
            return new ViOtpComWrapperSmsResult(false, new ViOtpComWrapperSms(res.Data));
        }
    }
    internal class ViOtpComWrapperSmsResult : List<ViOtpComWrapperSms>, IPhoneWrapperSmsResult<ViOtpComWrapperSms>
    {
        public ViOtpComWrapperSmsResult(bool isTimeout, ViOtpComWrapperSms wrapperSms)
        {
            this.IsTimeout = isTimeout;
            this.Add(wrapperSms);
        }
        public bool IsTimeout { get; }
    }
    internal class ViOtpComWrapperSms : IPhoneWrapperSms
    {
        readonly ViOtpComSessionGet viOtpComSessionGet;
        internal ViOtpComWrapperSms(ViOtpComSessionGet viOtpComSessionGet)
        {
            this.viOtpComSessionGet = viOtpComSessionGet;
        }
        public string Text => viOtpComSessionGet?.SmsContent;

        public string Code => viOtpComSessionGet?.Code;
    }
}
