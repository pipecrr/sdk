using AuditAppGrpcClient;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Siesa.SDK.Shared.Logs.DataEventLog
{
    public static class SDKLogger
    {
        public static void Configure(ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(new LoggerConfiguration()

                .MinimumLevel.Warning()
                .MinimumLevel.Override("Siesa.SDK.Business", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.StealthConsoleSink(logStorageService: new SDKGrpcLogStorageService("https://localhost:7168"))
                .CreateLogger());
        }
    }
}
