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
    public class TempMailOrgWrapper : IMailWrapper
    {
        TempMailOrgEndPoint _endPoint = TempMailOrg.Web2;
        public virtual int AccountAvailablesCount => -1;
        /// <summary>
        /// 
        /// </summary>
        public TempMailOrgEndPoint EndPoint
        {
            get { return _endPoint; }
            set
            {
                if (_endPoint == null) throw new ArgumentNullException(nameof(EndPoint));
                _endPoint = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Task<IMailWrapperAccount?> GetAccountAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IMailWrapperAccount?>(new TempMailOrgWrapperSession(EndPoint));
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




        internal class TempMailOrgWrapperSession : IMailWrapperAccount
        {
            readonly TempMailOrg tempMailOrg = new TempMailOrg();
            TempMailOrgToken token;
            internal TempMailOrgWrapperSession(TempMailOrgEndPoint endPoint)
            {
                tempMailOrg.EndPoint = endPoint;
            }

            public string Email => token.MailBox;

            public string Password => string.Empty;

            public Task DeleteAsync(CancellationToken cancellationToken = default)
            {
                return Task.FromResult(0);
            }

            public void Dispose()
            {
                tempMailOrg.Dispose();
            }

            public async Task<IEnumerable<IMailWrapperEmail>> GetMailsAsync(CancellationToken cancellationToken = default)
            {
                var messages = await tempMailOrg.Messages(token, cancellationToken).ConfigureAwait(false);
                var mesageDatas = new List<TempMailOrgMessageData>();
                foreach (var item in messages.Messages)
                {
                    mesageDatas.Add(await tempMailOrg.MessageData(token, item, cancellationToken).ConfigureAwait(false));
                }
                return mesageDatas.Select(x => new TempMailOrgWrapperMail(x));
            }

            public async Task<string> InitAsync(IProxyInfo? proxyInfo, CancellationToken cancellationToken = default)
            {
                token = await tempMailOrg.InitToken(cancellationToken).ConfigureAwait(false);
                return token.MailBox;
            }
        }
        internal class TempMailOrgWrapperMail : IMailWrapperEmail
        {
            readonly TempMailOrgMessageData messageData;
            internal TempMailOrgWrapperMail(TempMailOrgMessageData tempMailOrgMessageData)
            {
                this.messageData = tempMailOrgMessageData ?? throw new ArgumentNullException(nameof(tempMailOrgMessageData));
            }
            public string FromAddress => messageData.From;

            public string Subject => messageData.Subject;

            public string RawBody => messageData.BodyHtml;

            public string Code => string.Empty;

            public DateTime? ReceivedTime => messageData?.CreatedAt;
        }
    }
}
