using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace OrderConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            static IConfigurationBuilder BuilderAction(IConfigurationBuilder builder)
            {
                builder = builder.SetBasePath(Path.Combine(AppContext.BaseDirectory))
                    .AddJsonFile("appsettings.json", false, false);

                var env = Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");
                if (string.IsNullOrEmpty(env) == false)
                    builder = builder.AddJsonFile($"appsettings.{env}.json", false, false);

                return builder;
            }

            var host = CreateHostBuilder(args, (b) => BuilderAction(b)).Build();


            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, Action<IConfigurationBuilder> builderAction)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builderAction)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    var configBuilder = new ConfigurationBuilder();
                    builderAction(configBuilder);
                    var cRoot = configBuilder.Build();
                    var startUp = new Startup(cRoot);
                    startUp.ConfigureServices(services);
                });
        }
    }
}
