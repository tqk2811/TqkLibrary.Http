using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Captcha.Services
{
    /// <summary>
    /// https://anti-captcha.com/apidoc/image
    /// </summary>
    public class AntiCaptchaApi : BaseApi
    {
        private class CreateTaskJson
        {
            [JsonProperty("clientKey")]
            public string ClientKey { get; set; }

            [JsonProperty("task")]
            public AntiCaptchaTask Task { get; set; }

            [JsonProperty("softId")]
            public int? SoftId { get; set; }

            [JsonProperty("languagePool")]
            public string LanguagePool { get; set; } = "en";

            [JsonProperty("callbackUrl")]
            public string CallbackUrl { get; set; }
        }
        private class TaskResultJson
        {
            [JsonProperty("clientKey")]
            public string ClientKey { get; set; }

            [JsonProperty("taskId")]
            public long TaskId { get; set; }
        }


        private const string EndPoint = "https://api.anti-captcha.com";

        /// <summary>
        ///
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="ApiKey">ApiKey</param>
        public AntiCaptchaApi(string ApiKey) : base(ApiKey)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IAntiCaptchaTaskResponse> CreateTask(
            AntiCaptchaTask antiCaptchaTask,
            string languagePool = "en",
            CancellationToken cancellationToken = default)
        {
            return await Build()
                .WithUrlPostJson(
                    new UrlBuilder(EndPoint, "createTask"),
                    new CreateTaskJson
                    {
                        ClientKey = ApiKey,
                        Task = antiCaptchaTask,
                        LanguagePool = languagePool
                    })
                .ExecuteAsync<AntiCaptchaTaskResponse>(cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<AntiCaptchaTaskResultResponse> GetTaskResult(IAntiCaptchaTaskResponse task, CancellationToken cancellationToken = default)
            => Build()
               .WithUrlPostJson(
                   new UrlBuilder(EndPoint, "getTaskResult"),
                   new TaskResultJson
                   {
                       ClientKey = ApiKey,
                       TaskId = task.TaskId.Value
                   })
               .ExecuteAsync<AntiCaptchaTaskResultResponse>(cancellationToken);



#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public class AntiCaptchaTask
        {
            [JsonConverter(typeof(StringEnumConverter))]
            [JsonProperty("type")]
            public AntiCaptchaType Type { get; set; }

            [JsonProperty("websiteURL")]
            public string WebsiteUrl { get; set; }

            [JsonProperty("websiteKey")]
            public string WebsiteKey { get; set; }

            [JsonProperty("proxyType")]
            public string ProxyType { get; set; }

            [JsonProperty("proxyAddress")]
            public string ProxyAddress { get; set; }

            [JsonProperty("proxyPort")]
            public int? ProxyPort { get; set; }

            [JsonProperty("proxyLogin")]
            public string ProxyLogin { get; set; }

            [JsonProperty("proxyPassword")]
            public string ProxyPassword { get; set; }

            [JsonProperty("userAgent")]
            public string UserAgent { get; set; }

            [JsonProperty("cookies")]
            public string Cookies { get; set; }

            [JsonProperty("body")]
            public string Body { get; set; }

            [JsonProperty("funcaptchaApiJSSubdomain")]
            public string FunCaptchaApiJSSubDomain { get; set; }

            [JsonProperty("data")]
            public string Data { get; set; }

            [JsonProperty("websitePublicKey")]
            public string WebsitePublicKey { get; set; }

            [JsonProperty("gt")]
            public string GT { get; set; }

            [JsonProperty("challenge")]
            public string Challenge { get; set; }

            [JsonProperty("geetestApiServerSubdomain")]
            public string GeeTestApiServerSubdomain { get; set; }

            [JsonProperty("isInvisible")]
            public bool? IsInvisible { get; set; }
        }

        public interface IAntiCaptchaTaskResponse
        {
            string ErrorCode { get; }
            string ErrorDescription { get; }
            int ErrorId { get; }
            long? TaskId { get; }
        }

        internal class AntiCaptchaTaskResponse : IAntiCaptchaTaskResponse
        {
            [JsonProperty("errorId")]
            public int ErrorId { get; set; }

            [JsonProperty("errorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty("errorDescription")]
            public string ErrorDescription { get; set; }

            [JsonProperty("taskId")]
            public long? TaskId { get; set; }
        }

        public class AntiCaptchaTaskResultResponse
        {
            [JsonProperty("errorId")]
            public int ErrorId { get; set; }

            [JsonProperty("errorCode")]
            public string ErrorCode { get; set; }

            [JsonProperty("errorDescription")]
            public string ErrorDescription { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("solution")]
            public AntiCaptchaTaskSolutionResultResponse Solution { get; set; }

            [JsonProperty("cost")]
            public double? cost { get; set; }

            [JsonProperty("ip")]
            public string Ip { get; set; }

            [JsonProperty("createTime")]
            public long? CreateTime { get; set; }

            [JsonProperty("endTime")]
            public long? EndTime { get; set; }

            [JsonProperty("solveCount")]
            public int? SolveCount { get; set; }

            public bool IsComplete()
            {
                return Status == null || Status.Equals("ready");
            }
        }

        public class AntiCaptchaTaskSolutionResultResponse
        {
            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("url")]
            public string Url { get; set; }

            public string gRecaptchaResponse { get; set; }
        }

        /// <summary>
        /// https://anti-captcha.com/apidoc/image
        /// </summary>
        public enum AntiCaptchaType
        {
            ImageToTextTask,
            RecaptchaV2Task,
            RecaptchaV2TaskProxyless,
            RecaptchaV3TaskProxyless,
            RecaptchaV2EnterpriseTask,
            RecaptchaV2EnterpriseTaskProxyless,
            FunCaptchaTask,
            FunCaptchaTaskProxyless,
            GeeTestTask,
            GeeTestTaskProxyless,
            HCaptchaTask,
            HCaptchaTaskProxyless
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }



    /// <summary>
    /// 
    /// </summary>
    public static class AntiCaptchaHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmapBuffer"></param>
        /// <returns></returns>
        public static AntiCaptchaApi.AntiCaptchaTask AntiCaptchaImageToTextTask(this byte[] bitmapBuffer)
        {
            AntiCaptchaApi.AntiCaptchaTask task = new AntiCaptchaApi.AntiCaptchaTask();
            task.Type = AntiCaptchaApi.AntiCaptchaType.ImageToTextTask;
            task.Body = Convert.ToBase64String(bitmapBuffer);
            return task;
        }
    }
}