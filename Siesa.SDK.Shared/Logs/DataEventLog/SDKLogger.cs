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

                //.MinimumLevel.Warning()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Filter
                .ByExcluding(logEvent => 
                    logEvent.MessageTemplate.Text.Contains("No store type was specified for the decimal property"))
                .WriteTo.StealthConsoleSink(logStorageService: new SDKGrpcLogStorageService(serviceConfiguration.AuditServerUrl))
                .CreateLogger());
        }
    }
}
