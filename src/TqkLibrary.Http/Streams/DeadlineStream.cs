using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Streams
{
    /// <summary>
    /// Read-only wrapper over an HTTP response stream that applies a deadline (<c>clock</c>) to every read.
    /// <para>
    /// Why: with <c>ResponseHeadersRead</c>, <c>HttpClient.Timeout</c> only covers the headers. Reading the
    /// body afterwards is unprotected: a half-open connection leaves a token-less <c>ReadAsync</c> hanging
    /// forever. This class links the read token with the clock token, so an expired download throws instead.
    /// </para>
    /// <para>
    /// The clock expiring while nobody cancelled throws <see cref="TimeoutException"/>; the caller cancelling
    /// (through the read token or <c>callerToken</c>) lets <see cref="OperationCanceledException"/> through.
    /// <c>callerToken</c> is needed when the clock is linked from the caller's own token: cancelling it also
    /// fires the clock, and a read without token (bare <c>CopyToAsync(dest)</c>) cannot tell who cancelled.
    /// </para>
    /// <para>
    /// Only the async reads are protected; the sync <see cref="Read"/> merely refuses to start once the deadline passed.
    /// </para>
    /// <para>
    /// Ownership: disposes the wrapped stream and the <c>clock</c>. <c>callerToken</c> is not owned.
    /// </para>
    /// </summary>
    public sealed class DeadlineStream : Stream
    {
        readonly Stream _inner;
        readonly CancellationTokenSource _clock;
        readonly CancellationToken _callerToken;
        bool _disposed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="inner">Response stream; owned by this object from now on.</param>
        /// <param name="clock">Deadline of the read (already <c>CancelAfter</c>); owned by this object from now on.</param>
        /// <param name="callerToken">Caller token, to tell "caller cancelled" from "expired" when <paramref name="clock"/> is linked from it.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public DeadlineStream(Stream inner, CancellationTokenSource clock, CancellationToken callerToken = default)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _callerToken = callerToken;
        }

        /// <inheritdoc/>
        public override bool CanRead => _inner.CanRead;
        /// <inheritdoc/>
        public override bool CanSeek => false;
        /// <inheritdoc/>
        public override bool CanWrite => false;
        /// <inheritdoc/>
        public override long Length => throw new NotSupportedException();
        /// <inheritdoc/>
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // The sync path cannot be cancelled midway; at least do not start a read that is already expired.
            _callerToken.ThrowIfCancellationRequested();
            if (_clock.IsCancellationRequested)
                throw Expired(null);

            return _inner.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            try
            {
                if (!cancellationToken.CanBeCanceled)
                    return await _inner.ReadAsync(buffer, offset, count, _clock.Token).ConfigureAwait(false);

                using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _clock.Token);
                return await _inner.ReadAsync(buffer, offset, count, linked.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex) when (IsExpiredWithoutCancellation(cancellationToken))
            {
                throw Expired(ex);
            }
        }

#if NET5_0_OR_GREATER
        /// <inheritdoc/>
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!cancellationToken.CanBeCanceled)
                    return await _inner.ReadAsync(buffer, _clock.Token).ConfigureAwait(false);

                using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _clock.Token);
                return await _inner.ReadAsync(buffer, linked.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex) when (IsExpiredWithoutCancellation(cancellationToken))
            {
                throw Expired(ex);
            }
        }
#endif

        /// <inheritdoc/>
        public override void Flush() { }
        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        /// <inheritdoc/>
        public override void SetLength(long value) => throw new NotSupportedException();
        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        /// <summary>The clock fired while neither the caller nor the read cancelled: a real expiry.</summary>
        bool IsExpiredWithoutCancellation(CancellationToken readToken)
            => _clock.IsCancellationRequested
               && !_callerToken.IsCancellationRequested
               && !readToken.IsCancellationRequested;

        static TimeoutException Expired(Exception? inner)
            => new TimeoutException("The stream was not read completely before its deadline.", inner);

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                _inner.Dispose();
                _clock.Dispose();
            }

            base.Dispose(disposing);
        }

#if NET5_0_OR_GREATER
        /// <inheritdoc/>
        public override async ValueTask DisposeAsync()
        {
            // Deliberately NOT calling base.DisposeAsync(): Stream's default calls Dispose() and would
            // dispose _inner/_clock again. The _disposed flag makes repeated/mixed dispose calls no-ops.
            if (!_disposed)
            {
                _disposed = true;
                await _inner.DisposeAsync().ConfigureAwait(false);
                _clock.Dispose();
            }

            GC.SuppressFinalize(this);
        }
#endif
    }
}
