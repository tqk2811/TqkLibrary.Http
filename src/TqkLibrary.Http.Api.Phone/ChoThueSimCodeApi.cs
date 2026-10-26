using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TqkLibrary.Http.Api.Phone
{
    /// <summary>
    /// https://chothuesimcode.com/account/api
    /// </summary>
    public sealed class ChoThueSimCodeApi : BaseApi
    {
        private const string EndPoint = "https://chothuesimcode.com/api";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ApiKey"></param>
        public ChoThueSimCodeApi(string ApiKey) : base(ApiKey)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ChoThueSimBaseResult<ChoThueSimResponseCode, ChoThueSimAccountInfo>> GetAccountInfo(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint).WithParam("act", "account").WithParam("apik", ApiKey))
            .ExecuteAsync<ChoThueSimBaseResult<ChoThueSimResponseCode, ChoThueSimAccountInfo>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ChoThueSimBaseResult<ChoThueSimResponseCode, List<ChoThueSimAppInfo>>> GetAppRunning(CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint).WithParam("act", "app").WithParam("apik", ApiKey))
            .ExecuteAsync<ChoThueSimBaseResult<ChoThueSimResponseCode, List<ChoThueSimAppInfo>>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="carrier"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ChoThueSimBaseResult<ChoThueSimResponseCodeGetPhoneNumber, ChoThueSimPhoneNumberResult>> GetPhoneNumber(
            ChoThueSimAppInfo app, ChoThueSimCarrier? carrier = null, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint)
                .WithParam("act", "number")
                .WithParam("apik", ApiKey)
                .WithParam("appId", app.Id)
                .WithParamIfNotNull("carrier", carrier))
            .ExecuteAsync<ChoThueSimBaseResult<ChoThueSimResponseCodeGetPhoneNumber, ChoThueSimPhoneNumberResult>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="number"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ChoThueSimBaseResult<ChoThueSimResponseCodeGetPhoneNumber, ChoThueSimPhoneNumberResult>> GetPhoneNumber(int appId, string number, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint)
                .WithParam("act", "number")
                .WithParam("apik", ApiKey)
                .WithParam("appId", appId)
                .WithParam("number", number))
            .ExecuteAsync<ChoThueSimBaseResult<ChoThueSimResponseCodeGetPhoneNumber, ChoThueSimPhoneNumberResult>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumberResult"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ChoThueSimBaseResult<ChoThueSimResponseCodeMessage, ChoThueSimMessageResult>> GetMessage(ChoThueSimPhoneNumberResult phoneNumberResult, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint)
                .WithParam("act", "code")
                .WithParam("apik", ApiKey)
                .WithParam("id", phoneNumberResult.Id))
            .ExecuteAsync<ChoThueSimBaseResult<ChoThueSimResponseCodeMessage, ChoThueSimMessageResult>>(cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumberResult"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ChoThueSimBaseResult<ChoThueSimResponseCodeCancelMessage, ChoThueSimRefundInfo>> CancelGetMessage(ChoThueSimPhoneNumberResult phoneNumberResult, CancellationToken cancellationToken = default)
            => Build()
            .WithUrlGet(new UrlBuilder(EndPoint)
                .WithParam("act", "expired")
                .WithParam("apik", ApiKey)
                .WithParam("id", phoneNumberResult.Id))
            .ExecuteAsync<ChoThueSimBaseResult<ChoThueSimResponseCodeCancelMessage, ChoThueSimRefundInfo>>(cancellationToken);
    }




#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public enum ChoThueSimResponseCode
    {
        Success = 0,
        Error = 1,
    }

    public enum ChoThueSimResponseCodeGetPhoneNumber
    {
        Success = 0,
        WalletNotEnough = 1,
        AppNotExist = 2,
        PhoneNumberIsTemporarilyRunningOut = 3
    }

    public enum ChoThueSimResponseCodeMessage
    {
        Success = 0,
        Waitting = 1,
        Timeout = 2,
        InputIsCorrect = 3
    }

    public enum ChoThueSimResponseCodeCancelMessage
    {
        Success = 0,
        IdNotFound = 1,
        WasCanceled = 2
    }
    public class ChoThueSimRefundInfo
    {
        public double Balance { get; set; }
        public double Refund { get; set; }
    }
    public class ChoThueSimPhoneNumberResult
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string App { get; set; }
        public double Cost { get; set; }
        public double Balance { get; set; }
    }
    public class ChoThueSimMessageResult
    {
        public string SMS { get; set; }
        public string Code { get; set; }
        public double? Cost { get; set; }
        public bool? IsCall { get; set; }
        public string CallFile { get; set; }
        public string CallText { get; set; }
    }
    public enum ChoThueSimCarrier
    {
        Viettel, Mobi, Vina, VNMB
    }
    public class ChoThueSimBaseResult<T1, T2>
    {
        public T1 ResponseCode { get; set; }
        public string Msg { get; set; }
        public T2 Result { get; set; }
    }
    public class ChoThueSimAppInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Cost { get; set; }
    }
    public class ChoThueSimAccountInfo
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public double Balance { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}