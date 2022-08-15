using Gelf.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Reflection;

namespace ETicaretDiagnostics.Logging
{
    public static class LoggingBuilderExtensions
    {
        private static void AddETicaretLog(this ILoggingBuilder builder, IConfiguration configuration)
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            string instanceId = configuration.GetValue("AppSettings:InstanceId", "");

            NLog.GlobalDiagnosticsContext.Set("InstanceId", instanceId);
            NLog.GlobalDiagnosticsContext.Set("AssemblyVersion", version);

            builder.AddNLog(configuration);

            if (configuration.GetValue("AppSettings:DisableGelf", false)) return;
            builder.AddGelf(options =>
            {
                if (string.IsNullOrWhiteSpace(instanceId))
                    options.LogSource = AppDomain.CurrentDomain.FriendlyName;
                else
                    options.LogSource = $"{AppDomain.CurrentDomain.FriendlyName} ({instanceId})";

                options.AdditionalFieldsFactory = (level, eventId, exc) => new System.Collections.Generic.Dictionary<string, object>
                {
                    ["log_level"] = Enum.GetName<LogLevel>(level),
                    ["exception_type"] = exc?.GetType().ToString()
                };
            });
        }
        public static void AddETicaretLogging(this IServiceCollection services, LogLevel minLogLevel, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(minLogLevel);

                builder.AddETicaretLog(configuration);
            });
        }
    }
}