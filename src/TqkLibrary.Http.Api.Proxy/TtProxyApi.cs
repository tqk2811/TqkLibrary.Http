using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.Http.Api.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public class TtProxyApi : BaseApi
    {
        const string EndPoint = "https://api.ttproxy.com/v1/";
        readonly string secret;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="license"></param>
        /// <param name="secret"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TtProxyApi(string license, string secret) : base(license)
        {
            if (string.IsNullOrEmpty(secret)) throw new ArgumentNullException(nameof(secret));
            this.secret = secret;
        }

        string GenerateParameters()
        {
            long unix = DateTimeOffset.Now.ToUnixTimeSeconds();
            string sign = ApiKey + unix + secret;
            string sign_md5;
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(sign);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                sign_md5 = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
            return $"license={ApiKey}&time={unix}&sign={sign_md5}";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<TtProxyObtainResult>> Obtain(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet($"{EndPoint}obtain?{GenerateParameters()}")
            .ExecuteAsync<TtProxyResult<TtProxyObtainResult>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<TtProxyObtainResult>> Obtain(int count, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder($"{EndPoint}obtain?{GenerateParameters()}").WithParam("cnt", count))
            .ExecuteAsync<TtProxyResult<TtProxyObtainResult>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<List<string>>> WhiteListQuery(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder($"{EndPoint}whitelist/query?{GenerateParameters()}"))
            .ExecuteAsync<TtProxyResult<List<string>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<List<string>>> WhiteListExists(string ip, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder($"{EndPoint}whitelist/exists?{GenerateParameters()}").WithParam("ip", ip))
            .ExecuteAsync<TtProxyResult<List<string>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<List<string>>> WhiteListAdd(string ip, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder($"{EndPoint}whitelist/add?{GenerateParameters()}").WithParam("ip", ip))
            .ExecuteAsync<TtProxyResult<List<string>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<List<string>>> WhiteListDelete(string ip, CancellationToken cancellationToken = default)
             => Build()
            .WithUrlGet(new UrlBuilder($"{EndPoint}whitelist/del?{GenerateParameters()}").WithParam("ip", ip))
            .ExecuteAsync<TtProxyResult<List<string>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<List<string>>> WhiteListClear(string ip, CancellationToken cancellationToken = default)
             => Build()
            .WithUrlGet(new UrlBuilder($"{EndPoint}whitelist/clear?{GenerateParameters()}").WithParam("ip", ip))
            .ExecuteAsync<TtProxyResult<List<string>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<List<string>>> SubLicenseList(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder($"{EndPoint}subLicense/list?{GenerateParameters()}"))
            .ExecuteAsync<TtProxyResult<List<string>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<TtProxySubLicenseResult>> SubLicenseList(int? page = null, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder($"{EndPoint}subLicense/list?{GenerateParameters()}").WithParamIfNotNull("page", page))
            .ExecuteAsync<TtProxyResult<TtProxySubLicenseResult>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<TtProxySubLicenseCreateResult>> SubLicenseCreate(int? traffic = null, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder($"{EndPoint}subLicense/create?{GenerateParameters()}").WithParamIfNotNull("traffic", traffic))
            .ExecuteAsync<TtProxyResult<TtProxySubLicenseCreateResult>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<TtProxySubLicenseRenewResult>> SubLicenseRenew(int? traffic = null, string subLicenseKey = null, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder($"{EndPoint}subLicense/renew?{GenerateParameters()}")
                .WithParamIfNotNull("traffic", traffic)
                .WithParamIfNotNull("key", subLicenseKey))
            .ExecuteAsync<TtProxyResult<TtProxySubLicenseRenewResult>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task<TtProxyResult<TtProxySubLicenseRenewResult>> SubLicenseReclaim(string key = null, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder($"{EndPoint}subLicense/reclaim?{GenerateParameters()}").WithParamIfNotNull("key", key))
            .ExecuteAsync<TtProxyResult<TtProxySubLicenseRenewResult>>(cancellationToken);

    }




#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TtProxyObtainResult
    {
        [JsonProperty("todayObtain")]
        public int? TodayObtain { get; set; }

        [JsonProperty("ipLeft")]
        public int? IpLeft { get; set; }

        [JsonProperty("trafficLeft")]
        public long TrafficLeft { get; set; }

        [JsonProperty("proxies")]
        public List<string> Proxies { get; set; }
    }
    public class TtProxySubLicenseCreateResult
    {
        public List<TtProxySubLicenseLicenseResult> licenses { get; set; }
        public long? trafficLeft { get; set; }
    }
    public class TtProxySubLicenseLicenseResult
    {
        public int? id { get; set; }
        public string key { get; set; }
        public string secret { get; set; }
        public int? obtainLimit { get; set; }
        public long? trafficLeft { get; set; }
        public int? ipDuration { get; set; }
        public string remark { get; set; }
        public long? totalTraffic { get; set; }
        public int? ipUsed { get; set; }
        public DateTime? updated { get; set; }
        public DateTime? created { get; set; }
    }
    public class TtProxySubLicensePageResult
    {
        public int? current { get; set; }
        public int? last { get; set; }
        public int? limit { get; set; }
        public int? total { get; set; }
    }
    public class TtProxySubLicenseRenewResult
    {
        public TtProxySubLicenseLicenseResult license { get; set; }
        public long? trafficLeft { get; set; }
    }
    public class TtProxySubLicenseResult
    {
        [JsonProperty("page")]
        public TtProxySubLicensePageResult Page { get; set; }

        [JsonProperty("list")]
        public List<TtProxySubLicenseLicenseResult> List { get; set; }
    }
    public enum TtProxyErrorCode
    {
        OK = 0,
        Failed = 1000,
        UnknownError = 1001,
        ServerInternalError = 1002,
        UnsupportedOperations = 1003,
        SendMailError = 1005,
        WrongArguments = 1010,
        LackArguments = 1011,
        InvalidData = 1012,
        AlreadyCompleted = 1100,
        AlreadyExpired = 1102,
        AlreadyExisted = 1103,
        NoModification = 1104,
        TooManyItems = 1105,
        InvalidRequest = 1400,
        AuthFailed = 1401,
        Forbidden = 1403,
        NotFound = 1404,
        InvalidCaptcha = 1405,
        NoActivation = 1406,
        UserForbidden = 1407,
        WrongPassword = 1408,
        TooManyErrorsWithIncorrectPassword = 1409,
        TooManyRequests = 1429,
        AbnormalOrderStatus = 1601,
        PaymentFailure = 1610,
        AuthFailed2 = 2000,
        ReachesTheUpperLimit = 2001,
        TemporarilyExhausted = 2003,
        WrongArguments2 = 2004,
        CredentialExpires = 2005,
        Forbidden2 = 2006,
        TooManyWhitelists = 2010
    }
    public class TtProxyResult<T>
    {
        [JsonProperty("code")]
        public TtProxyErrorCode Code { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
