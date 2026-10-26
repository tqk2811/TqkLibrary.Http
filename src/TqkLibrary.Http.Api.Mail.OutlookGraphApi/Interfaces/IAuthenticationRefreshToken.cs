namespace TqkLibrary.Http.Api.Mail.OutlookGraphApi.Interfaces
{
    public interface IAuthenticationRefreshToken
    {
        string ClientId { get; }
        string RefreshToken { get; }
    }
}
