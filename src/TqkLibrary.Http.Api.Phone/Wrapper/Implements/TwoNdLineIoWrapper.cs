using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Phone.Wrapper.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class TwoNdLineIoWrapper : IPhoneWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        public int WaitPhoneTimeout { get; set; } = 90000;
        /// <summary>
        /// 
        /// </summary>
        public TwoLineIoService? Service { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TwoLineIoNetWorkId? NetWorkId { get; set; }





        readonly TwoNdLineIoApi _twoLineIoApi;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="twoLineIoApi"></param>
        public TwoNdLineIoWrapper(TwoNdLineIoApi twoLineIoApi)
        {
            this._twoLineIoApi = twoLineIoApi;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public TwoNdLineIoWrapper(string apiKey)
        {
            this._twoLineIoApi = new TwoNdLineIoApi(apiKey);
        }
        /// <summary>
        /// 
        /// </summary>
        ~TwoNdLineIoWrapper()
        {
            _twoLineIoApi.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _twoLineIoApi.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IPhoneWrapperSession> RentPhoneAsync(CancellationToken cancellationToken = default)
        {
            if (Service == null) throw new InvalidOperationException($"set {nameof(Service)} first");
            var res = await _twoLineIoApi.PurchaseOTP(Service, NetWorkId, null, cancellationToken).ConfigureAwait(false);
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(WaitPhoneTimeout))
            {
                while (true)
                {
                    await Task.Delay(1000, cancellationToken);
                    var check = await _twoLineIoApi.CheckOrder(res, cancellationToken).ConfigureAwait(false);

                    if (res.Status != 1 || check.Status != 1 || check.Data.StatusOrder != TwoLineIoStatusOrder.Wait)
                        return new TwoLineIoWrapperSession(_twoLineIoApi, res, check);
                    if (!string.IsNullOrWhiteSpace(check.Data?.Phone))
                        return new TwoLineIoWrapperSession(_twoLineIoApi, res, check);
                    if (cancellationTokenSource.IsCancellationRequested)
                        throw new TimeoutException($"WaitPhoneTimeout: {WaitPhoneTimeout}");
                }
            }
        }
    }

    internal class TwoLineIoWrapperSession : IPhoneWrapperSession
    {
        readonly TwoNdLineIoApi twoLineIoApi;
        readonly TwoLineIoPurchaseOtpResponse twoLineIoPurchaseOtpResponse;
        readonly TwoLineIoData<TwoLineIoOrderData> twoLineIoOrderData;
        internal TwoLineIoWrapperSession(
            TwoNdLineIoApi twoLineIoApi,
            TwoLineIoPurchaseOtpResponse twoLineIoPurchaseOtpResponse,
            TwoLineIoData<TwoLineIoOrderData> twoLineIoOrderData)
        {
            this.twoLineIoApi = twoLineIoApi;
            this.twoLineIoPurchaseOtpResponse = twoLineIoPurchaseOtpResponse;
            this.twoLineIoOrderData = twoLineIoOrderData;
        }


        public string PhoneNumber
        {
            get
            {
                if (string.IsNullOrWhiteSpace(twoLineIoOrderData?.Data?.Phone))
                    return string.Empty;
                if (twoLineIoOrderData!.Data.Phone.Length == 10 && twoLineIoOrderData.Data.Phone.StartsWith("0"))
                    return twoLineIoOrderData.Data.Phone.Substring(1);
                return twoLineIoOrderData.Data.Phone;
            }
        }

        public bool IsSuccess => twoLineIoOrderData.Data.StatusOrder == TwoLineIoStatusOrder.Wait;// twoLineIoOrderData?.Status == 1 && twoLineIoPurchaseOtpResponse?.Status == 1;

        public string? Message => twoLineIoOrderData?.Message;

        public Task CancelWaitSmsAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public async Task<IPhoneWrapperSmsResult<IPhoneWrapperSms>> GetSmsAsync(CancellationToken cancellationToken = default)
        {
            var order = await twoLineIoApi.CheckOrder(twoLineIoPurchaseOtpResponse, cancellationToken).ConfigureAwait(false);
            return new TwoLineIoWrapperSmsResult(order.Data.StatusOrder != TwoLineIoStatusOrder.Wait, new TwoLineIoWrapperSms(order.Data));
        }
    }
    internal class TwoLineIoWrapperSmsResult : List<TwoLineIoWrapperSms>, IPhoneWrapperSmsResult<TwoLineIoWrapperSms>
    {
        public TwoLineIoWrapperSmsResult(bool isTimeout, TwoLineIoWrapperSms wrapperSms)
        {
            this.IsTimeout = isTimeout;
            this.Add(wrapperSms);
        }
        public bool IsTimeout { get; }
    }
    internal class TwoLineIoWrapperSms : IPhoneWrapperSms
    {
        readonly TwoLineIoOrderData twoLineIoOrderData;
        internal TwoLineIoWrapperSms(TwoLineIoOrderData twoLineIoOrderData)
        {
            this.twoLineIoOrderData = twoLineIoOrderData;
        }

        public string Text => twoLineIoOrderData?.Message ?? string.Empty;

        public string Code => twoLineIoOrderData?.Code ?? string.Empty;
    }


}
