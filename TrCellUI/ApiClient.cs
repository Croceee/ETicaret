using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TrCellUI
{
    public partial class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpClientWithTimeOut;
        private IdentityServerTokenClient _identityServerClient;
        public readonly ConcurrentDictionary<string, string> tokenLibrary = new ConcurrentDictionary<string, string>();

        private Uri BaseEndpoint { get; set; }
        public ApiClient(Uri baseEndpoint, IdentityServerTokenClient identityServerClient, ILogger<ApiClientFactory> logger)
        {
            if (baseEndpoint == null)
            {
                throw new ArgumentNullException("baseEndpoint");
            }

            BaseEndpoint = baseEndpoint;
            _identityServerClient = identityServerClient;
            // this.logger = logger;
            _httpClient = new HttpClient();
            _httpClientWithTimeOut = new HttpClient();
            _httpClientWithTimeOut.Timeout = new TimeSpan(0, 10, 0);
        }


        private string GetGeneratedUserToken(string userId)
        {
            string token = "";
            if (tokenLibrary.ContainsKey(userId))
            {
                token = tokenLibrary[userId];
            }

            return token;
        }

        private async Task SetGeneratedToken(string userId, string userHash)
        {
            string JWTToken = GetGeneratedUserToken(userId);
            if (JWTToken == "")
            {
                await GenerateAndSetToken(userId, userHash);
            }
            else
            {
                _httpClient.SetBearerToken(JWTToken);
                _httpClientWithTimeOut.SetBearerToken(JWTToken);
            }
        }

        private async Task<string> GenerateAndSetToken(string userId, string userHash) //fullname olarak neden userHash alıyor ? 
        {
            string JWTToken = "";
            var tokenresponse = await _identityServerClient.GetTokenFromAPI();
            if (tokenresponse == null || tokenresponse.IsError)
            {
                JWTToken = "";
            }
            else
            {
                JWTToken = tokenresponse.AccessToken;
            }

            if (!tokenLibrary.TryAdd(userId, JWTToken))
            {
                tokenLibrary[userId] = JWTToken;
            }

            _httpClient.SetBearerToken(JWTToken);
            _httpClientWithTimeOut.SetBearerToken(JWTToken);
            return JWTToken;
        }

        public async Task<TransactionResult<T>> GetAsync<T>(Uri requestUrl, string userId,
         string userHash, int counter)
        {      
            //addHeaders();
            await SetGeneratedToken(userId, userHash);
            var response = await _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);

            TransactionResult<T> result = await GenerateTransactionResultCollection<T>(response);

            if (result.ErrorType != "unauthorized")
            {
                return result;
            }

            counter++;
            if (counter == 5)
            {
                return result;
            }
            else
            {
                string token = await GenerateAndSetToken(userId, userHash);
                if (token == "")
                {
                    result = GenerateErrorToken<T>();
                }
                else
                {
                    result = await GetAsync<T>(requestUrl, userId, userHash, counter);
                }
            }


            return result;
        }

        public TransactionResult<T> GenerateErrorToken<T>()
        {
          
            // _authenticationErrorCount++;
            var newResult = new TransactionResult<T>();
            newResult.ErrorType = "exception";
            newResult.Result = false;
            newResult.ErrorMessage = "authenticationerror: no token key generated";
            return newResult;
        }

        public async Task<TransactionResult<T>> GetFirstAsync<T>(Uri requestUrl, string userId,
        string userHash, int counter)
        {
            //addHeaders();
            await SetGeneratedToken(userId, userHash);
            var response = await _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);

            TransactionResult<T> result = await GenerateTransactionResultEntity<T>(response);


            if (result.ErrorType != "unauthorized")
            {
                return result;
            }

            counter++;
            if (counter == 5)
            {
                return result;
            }
            else
            {
                string token = await GenerateAndSetToken(userId, userHash);
                if (token == "")
                {
                    result = GenerateErrorToken<T>();
                }
                else
                {
                    result = await GetFirstAsync<T>(requestUrl, userId, userHash, counter);
                }
            }

            return result;
        }


        public async Task<TransactionResult<T>> PostAsync<T>(Uri requestUrl, T content, string userId,
        string userHash, int counter)
        {
            //addHeaders();
            await SetGeneratedToken(userId, userHash);
            var response = await _httpClient.PostAsync(requestUrl.ToString(), CreateHttpContent<T>(content));
            var data = await response.Content.ReadAsStringAsync();
            TransactionResult<T> result = await GenerateTransactionResult<T>(response);

            if (result.ErrorType != "unauthorized")
            {
                return result;
            }

            counter++;
            if (counter == 5)
            {
                return result;
            }
            else
            {
                string token = await GenerateAndSetToken(userId, userHash);
                if (token == "")
                {
                    result = GenerateErrorToken<T>();
                }
                else
                {
                    result = await PostAsync<T>(requestUrl, content, userId, userHash, counter);
                }
            }
            return result;

        }

        public async Task<TransactionResult<T>> PutAsync<T>(Uri requestUrl, T content, string userId,
        string userHash, int counter)
        {
            //addHeaders();
            await SetGeneratedToken(userId, userHash);
            var response = await _httpClient.PutAsync(requestUrl.ToString(), CreateHttpContent<T>(content));
            var data = await response.Content.ReadAsStringAsync();
            TransactionResult<T> result = await GenerateTransactionResult<T>(response);

            if (result.ErrorType != "unauthorized")
            {
                return result;
            }

            counter++;
            if (counter == 5)
            {
                return result;
            }
            else
            {
                string token = await GenerateAndSetToken(userId, userHash);
                if (token == "")
                {
                    result = GenerateErrorToken<T>();
                }
                else
                {
                    result = await PutAsync<T>(requestUrl, content, userId, userHash, counter);
                }
            }
            return result;

         
        }

        private async Task<TransactionResult<T>> GenerateTransactionResult<T>(
         HttpResponseMessage response)
        {
            var result = new TransactionResult<T>();
            var data = await response.Content.ReadAsStringAsync();


            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                result.Result = false;
                result.ErrorType = "unauthorized";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result.Result = true;
                result.Entity = JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                try
                {
                    result.Result = false;
                    result.ErrorMessage = data == "" ? "" : ((dynamic)(JsonConvert.DeserializeObject(data))).message;
                    result.ErrorType = response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                        ? "exception"
                        : "validation";
                }
                catch (Exception ex)
                {
                    result.Result = false;
                    result.ErrorMessage = "An unhandled exception occurred while processing the API request";
                    result.ErrorType = "exception";
                }
            }

            return result;
        }

        private async Task<TransactionResult<T>> GenerateTransactionResultCollection<T>(
           HttpResponseMessage response)
        {
            TransactionResult<T> result = new TransactionResult<T>();
            var data = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                result.Result = false;
                result.ErrorMessage = "unauthorized";
                result.ErrorType = "unauthorized";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result.Result = true;
                //zxc
                result.Entities = JsonConvert.DeserializeObject<IEnumerable<T>>(data);
            }
            else
            {
                try
                {
                    result.Result = false;
                    result.ErrorMessage = string.IsNullOrWhiteSpace(data)
                        ? string.Empty
                        : ((dynamic)(JsonConvert.DeserializeObject(data))).message;
                    result.ErrorType = response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                        ? "exception"
                        : "validation";
                }
                catch (Exception ex)
                {
                    result.Result = false;
                    result.ErrorMessage = "An unhandled exception occurred while processing the API request";
                    result.ErrorType = "exception";
                }
            }

            return result;
        }

        private async Task<TransactionResult<T>> GenerateTransactionResultEntity<T>(
            HttpResponseMessage response)
        {
            TransactionResult<T> result = new TransactionResult<T>();
            var data = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                result.Result = false;
                result.ErrorMessage = "unauthorized";
                result.ErrorType = "unauthorized";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result.Result = true;
                //zxc
                result.Entity = JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                try
                {
                    result.Result = false;
                    result.ErrorMessage = string.IsNullOrWhiteSpace(data)
                        ? string.Empty
                        : ((dynamic)(JsonConvert.DeserializeObject(data))).message;
                    result.ErrorType = response.StatusCode == System.Net.HttpStatusCode.InternalServerError
                        ? "exception"
                        : "validation";
                }
                catch (Exception ex)
                {
                    result.Result = false;
                    result.ErrorMessage = "An unhandled exception occurred while processing the API request";
                    result.ErrorType = "exception";
                }
            }

            return result;
        }
        public Uri CreateRequestUri(string relativePath, string queryString = "")
        {
            var endpoint = new Uri(BaseEndpoint, relativePath);
            var uriBuilder = new UriBuilder(endpoint);
            uriBuilder.Query = queryString;
            return uriBuilder.Uri;
        }

      
        private HttpContent CreateHttpContent<T>(T content)
        {
            


            var json = JsonConvert.SerializeObject(content);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static JsonSerializerSettings MicrosoftDateFormatSettings
        {
            get
            {
                return new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                };
            }
        }
    }
}
