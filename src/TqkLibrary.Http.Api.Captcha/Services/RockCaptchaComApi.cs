using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Captcha.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class RockCaptchaComApi : BaseApi
    {
        readonly string Endpoint = "https://api.rockcaptcha.com";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        public RockCaptchaComApi(string apiKey) : base(apiKey)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteKey">reCAPTCHA website key</param>
        /// <param name="siteUrl">Address of a target web page. Can be located anywhere on the web site, even in a member area. Our workers don't navigate there but simulate the visit instead</param>
        /// <param name="isInvisible">Specify whether or not Recaptcha is invisible. This will render an appropriate widget for our workers. default = false</param>
        /// <param name="action">Recaptcha's "action" value. Website owners use this parameter to define what users are doing on the page. Example: grecaptcha.execute('site_key', {action:'login_test'})</param>
        /// <param name="s">Value of 'data-s' parameter. Applies only to Recaptchas on Google web sites</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CreateTaskResponse<TaskDataResult>> CreateTaskRecaptchaV2Async(
            string siteKey,
            string siteUrl,
            bool isInvisible = false,
            string? action = null,
            string? s = null,
            CancellationToken cancellationToken = default)
            => Build()
                .WithUrlGet(new UrlBuilder(Endpoint, "recaptchav2")
                    .WithParam("apikey", ApiKey)
                    .WithParam("sitekey", siteKey)
                    .WithParam("siteurl", siteUrl)
                    .WithParam("invisible", isInvisible.ToString().ToLower())
                    .WithParamIfNotNull("action", action)
                    .WithParamIfNotNull("s", s))
                .ExecuteAsync<CreateTaskResponse<TaskDataResult>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<CreateTaskResponse<string>> CreateTaskImageToTextAsync(byte[] image, CancellationToken cancellationToken = default)
            => Build()
                .WithUrlPostJson(new UrlBuilder(Endpoint, "Recognition"), new ImageToText() { Apikey = ApiKey, Image = Convert.ToBase64String(image) })
                .ExecuteAsync<CreateTaskResponse<string>>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Response<T>> GetTaskResultAsync<T>(CreateTaskResponse<T> task, CancellationToken cancellationToken = default)
            => GetTaskResultAsync<T>(task.TaskId, cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<Response<T>> GetTaskResultAsync<T>(long taskId, CancellationToken cancellationToken = default)
            => Build()
                .WithUrlGet(new UrlBuilder(Endpoint, "getresult")
                    .WithParam("apikey", ApiKey)
                    .WithParam("taskId", taskId))
                .ExecuteAsync<Response<T>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="delay"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Response<T>> WaitUntilResultAsync<T>(CreateTaskResponse<T> task, int delay = 5000, CancellationToken cancellationToken = default)
        {
            Response<T>? response = null;
            do
            {
                await Task.Delay(delay, cancellationToken);
                response = await GetTaskResultAsync(task, cancellationToken);
            }
            while (response.Status == Status.PROCESSING || response.Status == Status.PENDING);
            return response;
        }


#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum Status
        {
            SUCCESS,
            ERROR,
            PENDING,
            PROCESSING,
        }
        class ImageToText
        {
            [JsonProperty("apikey")]
            public string? Apikey { get; set; }

            [JsonProperty("image")]
            public string? Image { get; set; }
        }


        public class Response
        {
            [JsonProperty("Code")]
            public int Code { get; set; }

            [JsonProperty("Message")]
            public string? Message { get; set; }
        }
        public class CreateTaskResponse : Response
        {
            [JsonProperty("TaskId")]
            public long TaskId { get; set; }
        }
        public class CreateTaskResponse<T> : CreateTaskResponse
        {

        }
        public class Response<T> : Response
        {
            [JsonProperty("Status")]
            public Status Status { get; set; }

            [JsonProperty("Data")]
            public T? Data { get; set; }
        }
        public class TaskDataResult
        {
            //[JsonProperty("gRecaptchaResponse")]
            //public string? GRecaptchaResponse { get; set; }

            [JsonProperty("Token")]
            public string? Token { get; set; }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
