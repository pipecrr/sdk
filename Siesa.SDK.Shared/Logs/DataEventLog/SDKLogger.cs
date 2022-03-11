using AuditAppGrpcClient;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Siesa.SDK.Shared.Configurations;
using Microsoft.Extensions.Logging;

namespace Siesa.SDK.Shared.Logs.DataEventLog
{
    public static class SDKLogger
    {
        public static void Configure(ILoggingBuilder loggingBuilder, ServiceConfiguration serviceConfiguration)
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
            loggingBuilder.AddSerilog(new LoggerConfiguration()

                .MinimumLevel.Warning()
                .MinimumLevel.Override("Siesa.SDK.Business", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.StealthConsoleSink(logStorageService: new SDKGrpcLogStorageService(serviceConfiguration.AuditServerUrl))
                .CreateLogger());
        }
    }
}
