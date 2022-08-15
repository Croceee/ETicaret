using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ETicaretDiagnostics.Logging
{
    public static class LoggingHostExtensions
    {
        public static IHost AddUnhandledExceptionLogging(this IHost host, LogLevel logLevel = LogLevel.Error)
        {
            var scope = host.Services.CreateScope();
            var logger = scope.ServiceProvider.GetService<ILogger>();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Exception ex = e.ExceptionObject as Exception;
                if (ex != null)
                    logger.Log(logLevel, exception: ex, message: "Unhandled Exception");
                else
                    logger.Log(logLevel, message: "Unhandled Exception");
            };
            return host;
        }
    }
}
