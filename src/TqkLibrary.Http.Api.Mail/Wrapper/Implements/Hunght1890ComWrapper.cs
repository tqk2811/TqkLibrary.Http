using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Mail.Services.TempMails;
using TqkLibrary.Http.Api.Mail.Wrapper;
using TqkLibrary.Http.Api.Proxy.Wrapper.Interfaces;
using TqkLibrary.Utils;

namespace TqkLibrary.Http.Api.Mail.Wrapper.Implements
{
    public class Hunght1890ComWrapper : IMailWrapper
    {
        readonly Hunght1890Com _hunght1890Com;
        readonly List<string> _domains = new List<string>();
        readonly Random _random
#if NET6_0_OR_GREATER
            = Random.Shared;//from net6
#else
            = new Random(DateTime.Now.GetHashCode());
#endif
        public virtual int AccountAvailablesCount => -1;
        public Hunght1890ComWrapper()
        {
            _hunght1890Com = new();
        }
        public Hunght1890ComWrapper(Hunght1890Com hunght1890Com)
        {
            this._hunght1890Com = hunght1890Com ?? throw new ArgumentNullException(nameof(hunght1890Com));
        }

        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            var configure = await _hunght1890Com.GetConfigureAsync(cancellationToken);
            _domains.Clear();
            if (configure.Domains is not null)
            {
                _domains.AddRange(configure.Domains.Where(x => !string.IsNullOrWhiteSpace(x)));
            }
        }

        protected virtual string GenerateEmail()
        {
            if (!_domains.Any()) throw new InvalidOperationException();
            string domain = _domains[_random.Next(_domains.Count)];
            string email = RandomStrings.RandomStringAndNum(8, 12);
            return $"{email}{domain}";
        }


        public Task<IMailWrapperAccount?> GetAccountAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IMailWrapperAccount?>(new MailWrapperSession(GenerateEmail(), _hunght1890Com));
        }

        public Task ReQueueAccountAsync(IMailWrapperAccount mailSession, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        class MailWrapperSession : IMailWrapperAccount
        {
            readonly Hunght1890Com _hunght1890Com;
            public MailWrapperSession(string email, Hunght1890Com hunght1890Com)
            {
                if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
                this.Email = email;
                this._hunght1890Com = hunght1890Com ?? throw new ArgumentNullException(nameof(hunght1890Com));
            }
            public void Dispose()
            {

            }


            public string Email { get; }

            public string Password => string.Empty;

            public Task<string> InitAsync(IProxyInfo? proxyInfo, CancellationToken cancellationToken = default)
            {
                return Task.FromResult<string>(this.Email);
            }
            public Task DeleteAsync(CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }


            public async Task<IEnumerable<IMailWrapperEmail>> GetMailsAsync(CancellationToken cancellationToken = default)
            {
                var mails = await _hunght1890Com.GetMailsAsync(Email, cancellationToken);
                List<MailWrapperEmail> mailWrapperEmails = new List<MailWrapperEmail>();
                foreach (var item in mails.Where(x => x is not null))
                {
                    mailWrapperEmails.Add(new MailWrapperEmail(item));
                }
                return mailWrapperEmails;
            }

            class MailWrapperEmail : IMailWrapperEmail
            {
                readonly Hunght1890Com.MailData _mailData;
                public MailWrapperEmail(Hunght1890Com.MailData mailData)
                {
                    this._mailData = mailData ?? throw new ArgumentNullException(nameof(mailData));
                }

                public string? FromAddress => _mailData.From;

                public string? Subject => _mailData.Subject;

                public string? RawBody => _mailData.Body;

                public string? Code => string.Empty;

                public DateTime? ReceivedTime
                {
                    get
                    {
                        if (DateTime.TryParseExact(
                            _mailData.Timestamp,
                            "YYYY-MM-dd HH:mm:ss",
                            CultureInfo.CurrentCulture,
                            DateTimeStyles.None,
                            out DateTime result
                            ))
                        {
                            return result;
                        }
                        return null;
                    }
                }
            }
        }
    }
}
