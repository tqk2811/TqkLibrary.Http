using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Captcha.Services
{
    public class MultibotInApi : BaseApi
    {
        const string Endpoint = "http://api.multibot.in/";
        public MultibotInApi(string apiKey) : base(apiKey)
        {

        }

        public Task<TaskCreateResponse> GetTaskResultAsync(TaskCreateResponse data, CancellationToken cancellationToken = default)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            return Build()
                .WithUrlGet(new UrlBuilder(Endpoint, "res.php")
                    .WithParam("key", ApiKey!)
                    .WithParam("id", data.Request!)
                    .WithParam("json", 1)
                )
                .ExecuteAsync<TaskCreateResponse>(cancellationToken);
        }
        public async Task<TaskCreateResponse> WaitTaskCompletedAsync(TaskCreateResponse data, int delay = 5000, CancellationToken cancellationToken = default)
        {
            while (true)
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                var data2 = await GetTaskResultAsync(data).ConfigureAwait(false);
                if (data2.Status == State.Error && "CAPCHA_NOT_READY".Equals(data2.Request, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                else
                {
                    return data2;
                }
            }
        }

        public Task<TaskCreateResponse> CreateHcapchaTokenTaskAsync(HcaptchaTokenTaskCreateData data, CancellationToken cancellationToken = default)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));
            return Build()
                .WithUrlGet(new UrlBuilder(Endpoint, "in.php")
                    .WithParam("key", ApiKey!)
                    .WithParam("method", "hcaptcha")
                    .WithParam("json", 1)
                    .WithParam("sitekey", data.SiteKey)
                    .WithParam("pageurl", data.PageUrl)
                    .WithParamIfNotNull("domain", data.Domain)
                    .WithParamIfNotNull("cookies", data.Cookies)
                    .WithParamIfNotNull("proxy", data.Proxy)
                )
                .ExecuteAsync<TaskCreateResponse>(cancellationToken);
        }


        public class HcaptchaTokenTaskCreateData
        {
            public required string SiteKey { get; init; }
            public required string PageUrl { get; init; }
            public string? Domain { get; set; }
            public string? Data { get; set; }
            public string? Cookies { get; set; }
            /// <summary>
            /// LOGIN:PASS@IP:PORT
            /// </summary>
            public string? Proxy { get; set; }
        }







        public enum State
        {
            Error = 0,
            Success = 1,
        }
        public class TaskCreateResponse
        {
            [JsonProperty("status")]
            public State Status { get; set; }

            [JsonProperty("request")]
            public string? Request { get; set; }
        }
    }
}
