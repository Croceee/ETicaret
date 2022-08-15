using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrCellUI;

namespace OrderConsumer
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            //var appSettings = _configuration.GetSection("AppSettings").Get<BatchSettings>();
            //services.AddSingleton(appSettings);
            var appSettings = Configuration.GetSection("AppSettings");
            services.Configure<ApplicationVariables>(appSettings);

            services.AddSingleton<ApiClientFactory>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddHttpClient<IdentityServerTokenClient>();
            #region Authentication

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie();
            #endregion
            services.AddSingleton<OrderConsumer>();
            //services.AddLogging(LogLevel.Information, _configuration);

            //services.AddTransient<HttpClient>();


            //services.AddWipelotLogging(LogLevel.Information, _configuration);
        }
    }
}
