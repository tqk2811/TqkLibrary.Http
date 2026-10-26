using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Captcha.Wrapper.Classes;

namespace TqkLibrary.Http.Api.Captcha.Wrapper.Interfaces
{
    public interface IHcaptchaTokenWrapper
    {
        Task<ICaptchaTask<CaptchaTaskTextResult>> CreateHcaptchaTokenTaskAsync(
            HcaptchaDataRequest request,
            CancellationToken cancellationToken = default
            );
    }
}
