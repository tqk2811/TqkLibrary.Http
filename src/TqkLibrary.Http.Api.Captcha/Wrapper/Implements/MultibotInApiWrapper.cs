using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Captcha.Services;
using TqkLibrary.Http.Api.Captcha.Wrapper.Classes;
using TqkLibrary.Http.Api.Captcha.Wrapper.Interfaces;

namespace TqkLibrary.Http.Api.Captcha.Wrapper.Implements
{
    public class MultibotInApiWrapper : IHcaptchaTokenWrapper
    {
        readonly MultibotInApi _multibotInApi;
        public MultibotInApiWrapper(string apiKey)
        {
            _multibotInApi = new MultibotInApi(apiKey);
        }
        public MultibotInApiWrapper(MultibotInApi multibotInApi)
        {
            this._multibotInApi = multibotInApi ?? throw new ArgumentNullException(nameof(multibotInApi));
        }

        public async Task<ICaptchaTask<CaptchaTaskTextResult>> CreateHcaptchaTokenTaskAsync(
            HcaptchaDataRequest request,
            CancellationToken cancellationToken = default
            )
        {
            var result = await _multibotInApi.CreateHcapchaTokenTaskAsync(new MultibotInApi.HcaptchaTokenTaskCreateData()
            {
                PageUrl = request.PageUrl,
                SiteKey = request.SiteKey,
                Cookies = request.Cookies,
                Data = request.Data,
                Domain = request.Domain,
                Proxy = request.Proxy,
            }, cancellationToken);
            return new HcaptchaTask(_multibotInApi, result);
        }


        class HcaptchaTask : ICaptchaTask<CaptchaTaskTextResult>
        {
            readonly MultibotInApi _multibotInApi;
            readonly MultibotInApi.TaskCreateResponse _response;
            public HcaptchaTask(MultibotInApi multibotInApi, MultibotInApi.TaskCreateResponse response)
            {
                this._multibotInApi = multibotInApi ?? throw new ArgumentNullException(nameof(multibotInApi));
                this._response = response ?? throw new ArgumentNullException(nameof(response));
            }
            public async Task<CaptchaTaskTextResult> GetTaskResultAsync(int delay = 2000, CancellationToken cancellationToken = default)
            {
                MultibotInApi.TaskCreateResponse response = await _multibotInApi.WaitTaskCompletedAsync(_response, delay, cancellationToken);
                if (response.Status == MultibotInApi.State.Success)
                {
                    return new CaptchaTaskTextResult()
                    {
                        IsSuccess = true,
                        Value = response.Request,
                    };
                }
                else
                {
                    return new CaptchaTaskTextResult()
                    {
                        IsSuccess = false,
                        ErrorMessage = response.Request,
                    };
                }
            }
        }
    }
}
