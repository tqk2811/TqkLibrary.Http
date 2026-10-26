using TqkLibrary.Http.Api.Mail.OutlookGraphApi.Interfaces;

namespace TqkLibrary.Http.Api.Mail.OutlookGraphApi.Classes
{
    public class AuthenticationRefreshToken : IAuthenticationRefreshToken
    {
        public required string RefreshToken { get; set; }
        public required string ClientId { get; set; }
    }
}
