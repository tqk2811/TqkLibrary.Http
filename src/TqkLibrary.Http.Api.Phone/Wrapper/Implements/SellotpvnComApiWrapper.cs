using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Phone.Wrapper;

namespace TqkLibrary.Http.Api.Phone.Wrapper.Implements
{
    public class SellotpvnComApiWrapper : IPhoneWrapper
    {
        readonly SellotpvnComApi sellotpvnComApi;
        public SellotpvnComApiWrapper(SellotpvnComApi.ApiEndPoint apiEndPoint, string token) : this(new SellotpvnComApi(apiEndPoint, token))
        {

        }
        public SellotpvnComApiWrapper(SellotpvnComApi sellotpvnComApi)
        {
            this.sellotpvnComApi = sellotpvnComApi ?? throw new ArgumentNullException(nameof(sellotpvnComApi));
        }

        public SellotpvnComApi.Serivce Service { get; set; }
        public SellotpvnComApi.NetworkProvider? NetworkProvider { get; set; }

        public void Dispose()
        {
            sellotpvnComApi.Dispose();
        }

        public async Task<IPhoneWrapperSession> RentPhoneAsync(CancellationToken cancellationToken = default)
        {
            if (Service is null) throw new InvalidOperationException($"{nameof(SellotpvnComApiWrapper)}.{nameof(Service)} musbe set");
            var response = await sellotpvnComApi.Order(Service, NetworkProvider, cancellationToken).ConfigureAwait(false);
            return new PhoneWrapperSession(sellotpvnComApi, response);
        }

        class PhoneWrapperSession : IPhoneWrapperSession
        {
            readonly SellotpvnComApi sellotpvnComApi;
            SellotpvnComApi.Response response;
            public PhoneWrapperSession(SellotpvnComApi sellotpvnComApi, SellotpvnComApi.Response response)
            {
                this.sellotpvnComApi = sellotpvnComApi ?? throw new ArgumentNullException(nameof(sellotpvnComApi));
                this.response = response ?? throw new ArgumentNullException(nameof(response));
            }
            public bool IsSuccess => response.Status == SellotpvnComApi.Status.Pending;

            public string PhoneNumber => response.PhoneNumber;

            public string Message
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(response.Message)) return response.Message;
                    return string.Empty;
                }
            }

            public Task CancelWaitSmsAsync(CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public async Task<IPhoneWrapperSmsResult<IPhoneWrapperSms>> GetSmsAsync(CancellationToken cancellationToken = default)
            {
                response = await sellotpvnComApi.GetOrder(response, cancellationToken);
                if (response.Status == SellotpvnComApi.Status.Failed)
                {
                    return new PhoneWrapperSmsResult() { IsTimeout = true };
                }
                if (!string.IsNullOrWhiteSpace(response.Content))
                {
                    return new PhoneWrapperSmsResult(new PhoneWrapperSms(response));
                }
                return new PhoneWrapperSmsResult();
            }

            class PhoneWrapperSmsResult : List<IPhoneWrapperSms>, IPhoneWrapperSmsResult<IPhoneWrapperSms>
            {
                public PhoneWrapperSmsResult()
                {

                }
                public bool IsTimeout { get; set; } = false;
                public PhoneWrapperSmsResult(PhoneWrapperSms phoneWrapperSms)
                {
                    this.Add(phoneWrapperSms ?? throw new ArgumentNullException(nameof(phoneWrapperSms)));
                }
            }

            class PhoneWrapperSms : IPhoneWrapperSms
            {
                readonly SellotpvnComApi.Response response;
                public PhoneWrapperSms(SellotpvnComApi.Response response)
                {
                    this.response = response ?? throw new ArgumentNullException(nameof(response));
                }
                public string Text => response.Content;

                public string Code => response.Otp;
            }
        }
    }
}
