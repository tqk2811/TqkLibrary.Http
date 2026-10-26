using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class WrapperStream : Stream
    {
        readonly IDisposable _disposable;
        readonly Stream _stream;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpResponseMessage"></param>
        /// <param name="stream"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public WrapperStream(IDisposable disposable, Stream stream)
        {
            this._disposable = disposable ?? throw new ArgumentNullException(nameof(disposable));
            this._stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }
        /// <summary>
        /// 
        /// </summary>
        ~WrapperStream()
        {
            Dispose(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _disposable.Dispose();
            _stream.Dispose();
            base.Dispose(disposing);
        }
        /// <inheritdoc/>
        public override bool CanRead => _stream.CanRead;
        /// <inheritdoc/>
        public override bool CanSeek => _stream.CanSeek;
        /// <inheritdoc/>
        public override bool CanWrite => _stream.CanWrite;
        /// <inheritdoc/>
        public override long Length => _stream.Length;
        /// <inheritdoc/>
        public override long Position { get => _stream.Position; set => _stream.Position = value; }
        /// <inheritdoc/>
        public override void Flush()
        {
            _stream.Flush();
        }
        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }
        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }
        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }
        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }


        //https://devblogs.microsoft.com/pfxteam/overriding-stream-asynchrony/
        //must overwite BeginRead/EndRead, BeginWrite/EndWrite for asynchronous 
        /// <inheritdoc/>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback = null, object? state = null)
        {
            return _stream.BeginRead(buffer, offset, count, callback, state);
        }
        /// <inheritdoc/>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback = null, object? state = null)
        {
            return _stream.BeginWrite(buffer, offset, count, callback, state);
        }
        /// <inheritdoc/>
        public override int EndRead(IAsyncResult asyncResult)
        {
            return _stream.EndRead(asyncResult);
        }
        /// <inheritdoc/>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            _stream.EndWrite(asyncResult);
        }
        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            return _stream.ReadAsync(buffer, offset, count, cancellationToken);
        }
        /// <inheritdoc/>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
        {
            return _stream.WriteAsync(buffer, offset, count, cancellationToken);
        }
        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken = default)
        {
            return _stream.FlushAsync(cancellationToken);
        }
#if NET5_0_OR_GREATER
        /// <inheritdoc/>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _stream.ReadAsync(buffer, cancellationToken);
        }
        /// <inheritdoc/>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _stream.WriteAsync(buffer, cancellationToken);
        }
#endif
    }
}