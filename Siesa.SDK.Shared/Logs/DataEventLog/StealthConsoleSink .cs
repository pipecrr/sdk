using AuditAppGrpcClient;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;
using Siesa.SDK.Shared.Services;
using System;

namespace Siesa.SDK.Shared.Logs.DataEventLog
{
    public class StealthConsoleSink : ILogEventSink
    {
        IFormatProvider _formatProvider;
        ISDKLogStorageService _logStorageService;
        IAuthenticationService _authService;

        

        public StealthConsoleSink(IFormatProvider formatProvider, ISDKLogStorageService logStorageService)
        {
            _formatProvider = formatProvider;
            _logStorageService = logStorageService;
            //_authService = authService;
        }

        public void Emit(LogEvent logEvent)
        {
            //SDKLogEvent _sdkLogEvent = JsonConvert.DeserializeObject<SDKLogEvent>(JsonConvert.SerializeObject(logEvent));

            //_sdkLogEvent = (SDKLogEvent)logEvent;
            //SDKLogEvent _sdkLogEvent = ActivatorUtilities.CreateInstance<SDKLogEvent>(_provider);
            //SDKLogEvent _sdkLogEvent = (SDKLogEvent)Convert.ChangeType(logEvent, typeof(SDKLogEvent));
            //_sdkLogEvent.SetAuthenticationService(_authService);
            var logString = JsonConvert.SerializeObject(logEvent);
            if (_logStorageService == null)
            {
                Console.WriteLine(logString);
                return;
            }
            _logStorageService.Save(logString);  
         
        }

        /*public void EmitSDK(SDKLogEvent logEvent)
        {
            var logString = JsonConvert.SerializeObject(logEvent);

            if (_logStorageService == null)
            {
                Console.WriteLine(logString);
                return;
            }

            _logStorageService.Save(logString);
        }*/


    }
}
