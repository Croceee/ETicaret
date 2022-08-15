using IdentityModel.Client;
using TrCellUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TrCellUI.Models;

namespace TrCellUI
{
    public class IdentityServerTokenClient
    {
        IOptions<ApplicationVariables> appsettings;
        private HttpContext httpcontext;
        bool useAuthentication;

        public IdentityServerTokenClient(HttpClient client, IOptions<ApplicationVariables> appsettings_, IHttpContextAccessor httpContext_)
        {
            Client = client;
            appsettings = appsettings_;
            httpcontext = httpContext_.HttpContext;
        }

        public HttpClient Client { get; }
        public TokenClientOptions Options { get; }

        public async Task<AuthTokenResponse> GetTokenFromAPI()
        {
            var user = appsettings.Value.WebApiUser;
            var password = appsettings.Value.WebApiPassword;

            var authModel = new JWTLoginModel()
            {
                Username = user,
                Password = password
            };

            var response = await Client.PostAsJsonAsync(appsettings.Value.WebApiUrl + "api/Authenticate/JwtAuth", authModel);
            
            if (response.IsSuccessStatusCode)
            {
                var accessToken = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return new AuthTokenResponse
                {
                    AccessToken = accessToken,
                    Code = response.StatusCode,
                    Exception = null,
                    IsError = false
                };
            }
            else
            {
                return new AuthTokenResponse
                {
                    AccessToken = null,
                    Code = response.StatusCode,
                    IsError = true,
                    Exception = response.ReasonPhrase
                };
            }
        }
    }

    public class AuthTokenResponse
    {
        public string AccessToken { get; set; }
        public HttpStatusCode Code { get; set; }
        public string Exception { get; set; }
        public bool IsError { get; set; }
    }
}
