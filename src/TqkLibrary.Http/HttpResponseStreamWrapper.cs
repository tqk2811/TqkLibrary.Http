using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http
{
    public class HttpResponseStreamWrapper : WrapperStream
    {
        readonly HttpResponseMessage _httpResponseMessage;
        public HttpResponseStreamWrapper(HttpResponseMessage httpResponseMessage, Stream stream) : base(httpResponseMessage, stream)
        {
            _httpResponseMessage = httpResponseMessage;
        }

        public override long Length => _httpResponseMessage?.Content?.Headers?.ContentLength ?? base.Length;
    }
}