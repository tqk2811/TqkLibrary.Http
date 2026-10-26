using MailKit.Net.Imap;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;
using MailKit.Security;
using MailKit;
using MailKit.Search;
using TqkLibrary.Http.Api.Mail.Wrapper;
using TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces;

namespace TqkLibrary.Http.Api.Mail.Wrapper.Implements
{
    /// <summary>
    /// 
    /// </summary>
    public class ImapWrapper : IMailWrapper
    {
        readonly string _host;
        readonly int _port;
        readonly Queue<ImapAccount> _imapAccounts;
        readonly AsyncLock _asyncLock = new AsyncLock();
        public virtual int AccountAvailablesCount => -1;
        /// <summary>
        /// 
        /// </summary>
        public int MaxMailTake { get; set; } = 20;
        /// <summary>
        /// 
        /// </summary>
        public SearchQuery SearchQuery { get; set; } = SearchQuery.All;
        /// <summary>
        /// 
        /// </summary>
        public event Action<ImapWrapper>? OnDequeue;
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ImapAccount> ImapAccounts { get { return _imapAccounts; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="imapAccounts"></param>
        public ImapWrapper(string host, int port, IEnumerable<ImapAccount> imapAccounts)
        {
            if (string.IsNullOrWhiteSpace(host)) throw new ArgumentNullException(nameof(host));
            this._host = host;
            this._port = port;
            this._imapAccounts = new Queue<ImapAccount>(imapAccounts);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IMailWrapperAccount?> GetAccountAsync(CancellationToken cancellationToken = default)
        {
            using (await _asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                if (_imapAccounts.Count == 0) return null;

                try
                {
                    var account = _imapAccounts.Dequeue();
                    return new ImapSession(_host, _port, account, this);
                }
                finally
                {
                    ThreadPool.QueueUserWorkItem((o) => OnDequeue?.Invoke(this));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailSession"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ReQueueAccountAsync(IMailWrapperAccount mailSession, CancellationToken cancellationToken = default)
        {
            if (mailSession is ImapSession imapSession)
            {
                using (await _asyncLock.LockAsync(cancellationToken).ConfigureAwait(false))
                {
                    _imapAccounts.Enqueue(imapSession.account);
                }
            }
        }



        /// <summary>
        /// 
        /// </summary>
        public class ImapAccount
        {
            /// <summary>
            /// 
            /// </summary>
            public required string UserName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public required string Password { get; set; }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $"{UserName}|{Password}";
            }
        }
        internal class ImapSession : IMailWrapperAccount
        {
            readonly string host;
            readonly int port;
            internal readonly ImapAccount account;
            readonly ImapWrapper imapWrapper;
            internal ImapSession(string host, int port, ImapAccount imapAccount, ImapWrapper imapWrapper)
            {
                this.host = host;
                this.port = port;
                this.account = imapAccount;
                this.imapWrapper = imapWrapper;
            }

            readonly ImapClient imapClient = new ImapClient();
            public string Email => account.UserName;
            public string Password => account.Password;

            public Task DeleteAsync(CancellationToken cancellationToken = default)
            {
                return imapClient.Inbox.CloseAsync(cancellationToken: cancellationToken);
            }
            public void Dispose()
            {
                imapClient.Dispose();
            }


            public async Task<IEnumerable<IMailWrapperEmail>> GetMailsAsync(CancellationToken cancellationToken = default)
            {
                var uids = await imapClient.Inbox.SearchAsync(imapWrapper.SearchQuery, cancellationToken).ConfigureAwait(false);
                var results = new List<IMailWrapperEmail>();
                foreach (var uid in uids.Take(imapWrapper.MaxMailTake))
                {
                    var message = await imapClient.Inbox.GetMessageAsync(uid, cancellationToken).ConfigureAwait(false);
                    results.Add(new IMapMail(message));
                }
                return results;
            }


            public async Task<string> InitAsync(IProxyInfo? proxyInfo, CancellationToken cancellationToken = default)
            {
                await imapClient.ConnectAsync(host, port, SecureSocketOptions.Auto, cancellationToken).ConfigureAwait(false);
                await imapClient.AuthenticateAsync(account.UserName, account.Password, cancellationToken).ConfigureAwait(false);
                await imapClient.Inbox.OpenAsync(FolderAccess.ReadOnly, cancellationToken).ConfigureAwait(false);
                return account.UserName;
            }
        }

        internal class IMapMail : IMailWrapperEmail
        {
            readonly MimeMessage mimeMessage;
            internal IMapMail(MimeMessage mimeMessage)
            {
                this.mimeMessage = mimeMessage ?? throw new ArgumentNullException(nameof(mimeMessage));
            }

            public string? FromAddress => mimeMessage.From?.First()?.Name;
            public string? Subject => mimeMessage.Subject;
            public string? RawBody => mimeMessage.HtmlBody;
            public string? Code => string.Empty;
            public DateTime? ReceivedTime => mimeMessage?.Date.DateTime;
        }
    }
}
