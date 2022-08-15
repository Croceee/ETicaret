using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TrCellUI
{
    public class ApiClientFactory
    {
        private static Uri apiUri;
        private static IdentityServerTokenClient identityServerClient;
        private static ILogger<ApiClientFactory> logger;
        private Lazy<ApiClient> restClient = new Lazy<ApiClient>(
          () => new ApiClient(apiUri, identityServerClient, logger),
          LazyThreadSafetyMode.ExecutionAndPublication);

        public ApiClientFactory( ILogger<ApiClientFactory> _logger, IdentityServerTokenClient identityserverClient_, IOptions<ApplicationVariables> appsettings_)
        {

            apiUri = new Uri(appsettings_.Value.WebApiUrl);
            identityServerClient = identityserverClient_;
            logger = _logger;
        }

        public ApiClient Instance
        {
            get
            {
                return restClient.Value;
            }
        }
    }
}
