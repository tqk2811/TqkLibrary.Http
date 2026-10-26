using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Other.Others
{
    public static class HttpChunkedHelper
    {
        public static Task<Stream> ReadAsStreamAsync(this HttpContent content, bool isChunked, int buffers = 1024 * 1024)
        {
            if (!isChunked)
            {
                return content.ReadAsStreamAsync();
            }
            else
            {
                var task = content.ReadAsStreamAsync().ContinueWith<Stream>((streamTask) =>
                {
                    int chunkSize = 0;
                    var outputStream = new MemoryStream();
                    var buffer = new char[buffers];
                    var stream = streamTask.Result;
                    // No using() so that we don't dispose stream.
                    var tr = new StreamReader(stream);
                    var tw = new StreamWriter(outputStream);
                    while (!tr.EndOfStream)
                    {
                        var chunkSizeStr = tr.ReadLine().Trim();
                        if (int.TryParse(chunkSizeStr, System.Globalization.NumberStyles.HexNumber, CultureInfo.CurrentCulture, out chunkSize))
                        {
                            tr.ReadBlock(buffer, 0, chunkSize);
                            tw.Write(buffer, 0, chunkSize);
                            tr.ReadLine();
                        }
                        else
                        {
                            tw.Write(chunkSizeStr);
                        }
                    }
                    return outputStream;
                });

                return task;
            }
        }
    }
}