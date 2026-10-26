using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.HttpClientHandles
{
    public class SynchronizationRequestHandler : DelegatingHandler
    {
        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        public SynchronizationRequestHandler(HttpMessageHandler innerHandler) : base(innerHandler ?? throw new ArgumentNullException(nameof(innerHandler)))
        {

        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _semaphore.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
