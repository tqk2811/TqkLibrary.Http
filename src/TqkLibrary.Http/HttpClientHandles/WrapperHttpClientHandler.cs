using System;
using System.Net.Http;

namespace TqkLibrary.Http.HttpClientHandles
{
    /// <summary>
    /// 
    /// </summary>
    public class WrapperHttpClientHandler : HttpClientHandler
    {
        /// <summary>
        /// Disable dispose
        /// </summary>
        /// <param name="disposing"></param>
        /// <exception cref="InvalidOperationException"></exception>
        protected override void Dispose(bool disposing)
        {
            //throw new InvalidOperationException("WrapperHttpClientHandler can't dispose");
        }
    }
}
