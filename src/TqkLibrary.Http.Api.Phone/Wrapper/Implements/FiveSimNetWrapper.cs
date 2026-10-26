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
    public class FiveSimNetWrapper : IPhoneWrapper
    {
        /// <summary>
        /// default: any
        /// </summary>
        public string Country { get; set; } = "any";
        /// <summary>
        /// default: any
        /// </summary>
        public string Operator { get; set; } = "any";
        /// <summary>
        /// 
        /// </summary>
        public string Product { get; set; }


        readonly FiveSimNetApi fiveSimNetApi;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public FiveSimNetWrapper(string apiKey)
        {
            this.fiveSimNetApi = new FiveSimNetApi(apiKey);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fiveSimNetApi"></param>
        public FiveSimNetWrapper(FiveSimNetApi fiveSimNetApi)
        {
            this.fiveSimNetApi = fiveSimNetApi;
        }
        /// <summary>
        /// 
        /// </summary>
        ~FiveSimNetWrapper()
        {
            fiveSimNetApi.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            fiveSimNetApi.Dispose();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IPhoneWrapperSession> RentPhoneAsync(CancellationToken cancellationToken = default)
        {
            var res = await fiveSimNetApi.BuyActivationNumber(Country, Operator, Product, cancellationToken).ConfigureAwait(false);
            return new FiveSimNetWrapperSession(fiveSimNetApi, res);
        }
    }

    internal class FiveSimNetWrapperSession : IPhoneWrapperSession
    {
        readonly FiveSimNetApi fiveSimNetApi;
        readonly FiveSimNetNumber fiveSimNetNumber;
        internal FiveSimNetWrapperSession(FiveSimNetApi fiveSimNetApi, FiveSimNetNumber fiveSimNetNumber)
        {
            this.fiveSimNetApi = fiveSimNetApi;
            this.fiveSimNetNumber = fiveSimNetNumber;
        }
        public string PhoneNumber => fiveSimNetNumber?.Phone;

        public bool IsSuccess => fiveSimNetNumber.Status == FiveSimNetOrderStatuses.PENDING;

        public string Message => string.Empty;

        public Task CancelWaitSmsAsync(CancellationToken cancellationToken = default)
        {
            return fiveSimNetApi.CancelOrder(fiveSimNetNumber, cancellationToken);
        }

        public async Task<IPhoneWrapperSmsResult<IPhoneWrapperSms>> GetSmsAsync(CancellationToken cancellationToken = default)
        {
            var res = await fiveSimNetApi.CheckOrder(fiveSimNetNumber, cancellationToken).ConfigureAwait(false);
            return new FiveSimNetWrapperSmsResult(
                res.Status != FiveSimNetOrderStatuses.PENDING && res.Status != FiveSimNetOrderStatuses.FINISHED,
                res.Sms.Select(x => new FiveSimNetWrapperSms(x)));
        }
    }
    internal class FiveSimNetWrapperSmsResult : List<FiveSimNetWrapperSms>, IPhoneWrapperSmsResult<FiveSimNetWrapperSms>
    {
        public FiveSimNetWrapperSmsResult(bool isTimeout, IEnumerable<FiveSimNetWrapperSms> wrapperSms)
        {
            this.IsTimeout = isTimeout;
            foreach (var item in wrapperSms)
            {
                this.Add(item);
            }
        }
        public bool IsTimeout { get; }
    }
    internal class FiveSimNetWrapperSms : IPhoneWrapperSms
    {
        readonly FiveSimNetSms fiveSimNetSms;
        internal FiveSimNetWrapperSms(FiveSimNetSms fiveSimNetSms)
        {
            this.fiveSimNetSms = fiveSimNetSms;
        }
        public string Text => fiveSimNetSms?.Text;

        public string Code => fiveSimNetSms?.Code;
    }
}
