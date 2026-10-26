//using Nito.AsyncEx;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace TqkLibrary.Http.Mails.TempMails.Wrapper.Implements
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class Pop3Wrapper : IMailWrapper
//    {
//        readonly string host;
//        readonly int port;
//        readonly Queue<ImapAccount> imapAccounts;
//        readonly AsyncLock asyncLock = new AsyncLock();
//        public virtual int AccountAvailablesCount => -1;
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="host"></param>
//        /// <param name="port"></param>
//        /// <param name="imapAccounts"></param>
//        public Pop3Wrapper(string host, int port, IEnumerable<ImapAccount> imapAccounts)
//        {

//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="login"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<IMailWrapperSession> CreateSessionAsync(string login, CancellationToken cancellationToken = default)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="mailSession"></param>
//        /// <param name="cancellationToken"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task ReQueueSessionAsync(IMailWrapperSession mailSession, CancellationToken cancellationToken = default)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    internal class Pop3Session : IMailWrapperSession
//    {
//        public string Email => throw new NotImplementedException();

//        public string Password => throw new NotImplementedException();

//        public Task DeleteAsync(CancellationToken cancellationToken = default)
//        {
//            throw new NotImplementedException();
//        }

//        public void Dispose()
//        {
//            throw new NotImplementedException();
//        }

//        public Task<IEnumerable<IMailWrapperEmail>> GetMailsAsync(CancellationToken cancellationToken = default)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<string> InitAsync(IProxyInfo? proxyInfo,CancellationToken cancellationToken = default)
//        {
//            throw new NotImplementedException();
//        }
//    }
//    internal class Pop3Mail : IMailWrapperEmail
//    {
//        public string FromAddress => throw new NotImplementedException();
//        public string Subject => throw new NotImplementedException();
//        public string RawBody => throw new NotImplementedException();
//    }
//}
