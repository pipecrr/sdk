using AuditAppGrpcClient;
using Serilog;
using Serilog.Configuration;
using System;

namespace Siesa.SDK.Shared.Logs.DataEventLog
{
    

    public static class StealthConsoleSinkExtensions
    {
        public static LoggerConfiguration StealthConsoleSink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  IFormatProvider fmtProvider = null, ISDKLogStorageService logStorageService = null)
        {
            return loggerConfiguration.Sink(new StealthConsoleSink(fmtProvider, logStorageService));
        }
    }
}
