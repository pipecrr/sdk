using AuditAppGrpcClient;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;
using System;

namespace Siesa.SDK.Shared.Logs.DataEventLog
{
    public class StealthConsoleSink : ILogEventSink
    {
        IFormatProvider _formatProvider;
        ISDKLogStorageService _logStorageService;
        

        public StealthConsoleSink(IFormatProvider formatProvider, ISDKLogStorageService logStorageService)
        {
            _formatProvider = formatProvider;
            _logStorageService = logStorageService;
        }

        public void Emit(LogEvent logEvent)
        {
            var logString = JsonConvert.SerializeObject(logEvent);
            if (_logStorageService == null)
            {
                Console.WriteLine(logString);
                return;
            }
            _logStorageService.Save(logString);  
         
        }


    }
}
