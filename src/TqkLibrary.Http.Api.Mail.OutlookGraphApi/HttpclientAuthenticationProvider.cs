using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Newtonsoft.Json;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Http.Api.Mail.OutlookGraphApi.Classes;
using TqkLibrary.Http.Api.Mail.OutlookGraphApi.Interfaces;

namespace TqkLibrary.Http.Api.Mail.OutlookGraphApi
{
    public class HttpclientAuthenticationProvider : IAuthenticationProvider, IDisposable
    {
        public AuthenticationResponse? Authentication { get; private set; }
        readonly AsyncLock _asyncLock = new AsyncLock();
        readonly IAuthenticationRefreshToken _authenticationRefreshToken;
        readonly HttpMessageHandler _httpMessageHandler;
        readonly bool _disposeHandler;
        readonly HttpClient _httpClient;
        public HttpclientAuthenticationProvider(IAuthenticationRefreshToken authenticationRefreshToken)
            : this(authenticationRefreshToken, NetSingleton.HttpClientHandler, false)
        {

        }
        public HttpclientAuthenticationProvider(
            IAuthenticationRefreshToken authenticationRefreshToken,
            HttpMessageHandler httpMessageHandler,
            bool disposeHandler = true
            )
        {
            this._authenticationRefreshToken = authenticationRefreshToken ?? throw new ArgumentNullException(nameof(authenticationRefreshToken));
            this._httpMessageHandler = httpMessageHandler ?? throw new ArgumentNullException(nameof(httpMessageHandler));
            this._disposeHandler = disposeHandler;
            this._httpClient = new HttpClient(httpMessageHandler, disposeHandler);
        }
        ~HttpclientAuthenticationProvider()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        void Dispose(bool disposing)
        {
            _httpClient.Dispose();
        }

        public async Task<AuthenticationResponse> ForceRefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            using (var l = await _asyncLock.LockAsync(cancellationToken))
            {
                Authentication = await _RefreshTokenAsync(cancellationToken);
                return Authentication;
            }
        }

        public async Task AuthenticateRequestAsync(
            RequestInformation request,
            Dictionary<string, object>? additionalAuthenticationContext = null,
            CancellationToken cancellationToken = default
            )
        {
            using (var l = await _asyncLock.LockAsync(cancellationToken))
            {
                if (Authentication is null || Authentication.IsExpired)
                {
                    Authentication = await _RefreshTokenAsync(cancellationToken);
                }
            }
            request.Headers.Add("Authorization", $"{Authentication.TokenType} {Authentication.AccessToken}");
        }




        async Task<AuthenticationResponse> _RefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            var values = new Dictionary<string, string>
            {
                { "client_id", _authenticationRefreshToken.ClientId },
                { "grant_type", "refresh_token" },
                { "refresh_token", _authenticationRefreshToken.RefreshToken },
                { "scope", "https://graph.microsoft.com/.default" }
            };
            using var content = new FormUrlEncodedContent(values);
            using var response = await _httpClient.PostAsync("https://login.microsoftonline.com/consumers/oauth2/v2.0/token", content, cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<AuthenticationResponse>(responseString)!;
        }
    }
}
