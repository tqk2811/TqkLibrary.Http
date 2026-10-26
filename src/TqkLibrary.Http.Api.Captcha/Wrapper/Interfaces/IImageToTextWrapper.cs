using System;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Captcha.Wrapper.Classes;

namespace TqkLibrary.Http.Api.Captcha.Wrapper.Interfaces
{
    public interface IImageToTextWrapper
    {
        Task<ICaptchaTask<CaptchaTaskTextResult>> CreateImageToTextTaskAsync(
            byte[] bitmapBuffer,
            CancellationToken cancellationToken = default);

    }
}
