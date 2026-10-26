using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Mail.Services.TempMails;
using TqkLibrary.Http.Api.Mail.Wrapper;
using TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces;

namespace TqkLibrary.Http.Api.Mail.Wrapper.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class MailTmTempWrapper : IMailWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        public class TmAccount
        {
            /// <summary>
            /// 
            /// </summary>
            public required string UserName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Password { get; set; } = "sdfw2szfw3qr";

            internal void Check()
            {
                if (string.IsNullOrWhiteSpace(UserName)) throw new ArgumentNullException(nameof(UserName));
                if (string.IsNullOrWhiteSpace(Password)) throw new ArgumentNullException(nameof(Password));
            }
        }


        public virtual int AccountAvailablesCount => -1;

        readonly Func<TmAccount> _accountCallback;
        /// <summary>
        /// 
        /// </summary>
        public MailTmTempWrapper(Func<TmAccount> accountCallback)
        {
            this._accountCallback = accountCallback ?? throw new ArgumentNullException(nameof(accountCallback));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<IMailWrapperAccount?> GetAccountAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IMailWrapperAccount?>(new MailTmTempManagedSession(_accountCallback()));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailSession"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task ReQueueAccountAsync(IMailWrapperAccount mailSession, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }



        internal class MailTmTempManagedSession : IMailWrapperAccount
        {
            readonly Random _random = new Random();
            readonly MailTmApi _mailTmApi = new MailTmApi();
            readonly TmAccount _tmAccount;
            MailTmToken? _token;
            MailTmAccount? _mailTmAccount;
            MailTmAccountResponse? _mailTmAccountResponse;
            readonly Dictionary<string, MailTmMessageData> _dict_messageDatas = new Dictionary<string, MailTmMessageData>();

            internal MailTmTempManagedSession(TmAccount tmAccount)
            {
                this._tmAccount = tmAccount ?? throw new ArgumentNullException(nameof(tmAccount));
                this._tmAccount.Check();
            }
            ~MailTmTempManagedSession()
            {
                _mailTmApi.Dispose();
            }
            public void Dispose()
            {
                _mailTmApi.Dispose();
                GC.SuppressFinalize(this);
            }
            public string Email => _mailTmAccount?.Address!;

            public string Password => string.Empty;

            public async Task<IEnumerable<IMailWrapperEmail>> GetMailsAsync(CancellationToken cancellationToken = default)
            {
                if (_token == null) throw new InvalidOperationException();

                var messages = await _mailTmApi.Messages(_token, 1, cancellationToken).ConfigureAwait(false);
                var messageDatas = new List<MailTmMessageData>();
                foreach (var message in messages.Members.Where(x => !_dict_messageDatas.ContainsKey(x.Id)))
                {
                    messageDatas.Add(await _mailTmApi.Message(_token, message, cancellationToken).ConfigureAwait(false));
                    _dict_messageDatas[message.Id] = messageDatas.Last();
                }
                return messages.Members.Select(x => new MailTmTempManagedMail(_dict_messageDatas[x.Id]));
            }

            public async Task<string> InitAsync(IProxyInfo? proxyInfo, CancellationToken cancellationToken = default)
            {
                if (_token != null) throw new InvalidOperationException();
                var domains = await _mailTmApi.Domains().ConfigureAwait(false);
                var domain = domains.Members[_random.Next(domains.Members.Count)];
                _mailTmAccount = new MailTmAccount(_tmAccount.UserName, domain, _tmAccount.Password);
                _mailTmAccountResponse = await _mailTmApi.AccountCreate(_mailTmAccount, cancellationToken).ConfigureAwait(false);
                _token = await _mailTmApi.Token(_mailTmAccount, cancellationToken).ConfigureAwait(false);
                return _mailTmAccount.Address;
            }

            public async Task DeleteAsync(CancellationToken cancellationToken = default)
            {
                if (_token == null || _mailTmAccountResponse == null) throw new InvalidOperationException();
                await _mailTmApi.AccountsDelete(_mailTmAccountResponse, _token, cancellationToken).ConfigureAwait(false);
            }
        }

        internal class MailTmTempManagedMail : IMailWrapperEmail
        {
            readonly MailTmMessageData mailTmMessageData;
            public MailTmTempManagedMail(MailTmMessageData mailTmMessageData)
            {
                this.mailTmMessageData = mailTmMessageData ?? throw new ArgumentNullException(nameof(mailTmMessageData));
            }

            public string? FromAddress
            {
                get
                {
                    return mailTmMessageData.From?.Address;
                }
            }

            public string? Subject
            {
                get
                {
                    return mailTmMessageData.Subject;
                }
            }

            public string? RawBody
            {
                get
                {
                    return string.Join("\r\n\r\n", mailTmMessageData.Html);
                }
            }
            public string? Code => string.Empty;

            public DateTime? ReceivedTime => mailTmMessageData?.RetentionDate;
        }
    }
}
