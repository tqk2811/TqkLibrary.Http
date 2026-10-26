using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Captcha.Wrapper.Interfaces
{
    public interface ICaptchaTask<TTaskResult>
    {
        Task<TTaskResult> GetTaskResultAsync(int delay = 2000, CancellationToken cancellationToken = default);
    }
}
