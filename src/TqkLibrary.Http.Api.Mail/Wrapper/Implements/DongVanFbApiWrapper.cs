using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Mail.Services.BuyMailApi;
using TqkLibrary.Http.Api.Mail.Wrapper;
using TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces;

namespace TqkLibrary.Http.Api.Mail.Wrapper.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class DongVanFbApiWrapper : IMailWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        public DongVanFbApi.DongVanFbAccountType? AccountType { get; set; }
        /// <summary>
        /// Default 1
        /// </summary>
        public int Amount { get; } = 1;

        public virtual int AccountAvailablesCount => -1;

        readonly DongVanFbApi _dongVanFbApi;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dongVanFbApi"></param>
        public DongVanFbApiWrapper(DongVanFbApi dongVanFbApi)
        {
            this._dongVanFbApi = dongVanFbApi;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public DongVanFbApiWrapper(string apiKey)
        {
            this._dongVanFbApi = new DongVanFbApi(apiKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IMailWrapperAccount?> GetAccountAsync(CancellationToken cancellationToken = default)
        {
            if (AccountType == null) throw new InvalidOperationException($"{AccountType} was null");
            var acc = await _dongVanFbApi.BuyMail(AccountType, Amount, cancellationToken).ConfigureAwait(false);


            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailSession"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task ReQueueAccountAsync(IMailWrapperAccount mailSession, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }



        internal class DongVanFbApiWrapperSession : IMailWrapperAccount
        {
            readonly DongVanFbApi _dongVanFbApi;
            readonly DongVanFbApi.DongVanFbBuyMailResponse _dongVanFbBuyMailResponse;
            readonly DongVanFbApi.DongVanFbMailAccount _account;
            internal DongVanFbApiWrapperSession(DongVanFbApi dongVanFbApi, DongVanFbApi.DongVanFbBuyMailResponse dongVanFbBuyMailResponse)
            {
                this._dongVanFbApi = dongVanFbApi;
                this._dongVanFbBuyMailResponse = dongVanFbBuyMailResponse;
                this._account = dongVanFbBuyMailResponse.Data!.ListDataAccount!.First();
            }
            public string Email => _account.Email ?? throw new NullReferenceException($"Email");
            public string Password => _account.Password ?? throw new NullReferenceException($"Password");

            public Task DeleteAsync(CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public void Dispose()
            {

            }

            public async Task<IEnumerable<IMailWrapperEmail>> GetMailsAsync(CancellationToken cancellationToken = default)
            {
                var messages = await _dongVanFbApi.GetMessages(_account, cancellationToken).ConfigureAwait(false);
                return messages.Messages?.Select(x => new DongVanFbApiWrapperEmail(x)) ?? Enumerable.Empty<IMailWrapperEmail>();
            }

            public Task<string> InitAsync(IProxyInfo? proxyInfo, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(string.Empty);
            }
        }

        internal class DongVanFbApiWrapperEmail : IMailWrapperEmail
        {
            readonly DongVanFbApi.DongVanFbMessage _dongVanFbMessage;
            internal DongVanFbApiWrapperEmail(DongVanFbApi.DongVanFbMessage dongVanFbMessage)
            {
                this._dongVanFbMessage = dongVanFbMessage;
            }
            public string? FromAddress => _dongVanFbMessage?.From?.FirstOrDefault()?.Address;

            public string? Subject => _dongVanFbMessage?.Subject;

            public string? RawBody => _dongVanFbMessage?.Message;

            public string? Code => _dongVanFbMessage?.Code;

            public DateTime? ReceivedTime => _dongVanFbMessage?.Date;
        }
    }
}
