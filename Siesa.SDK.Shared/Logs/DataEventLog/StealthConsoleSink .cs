using AuditAppGrpcClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Serilog.Core;
using Serilog.Events;
using System;
using System.IO;

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
            /*MemoryStream ms = new MemoryStream();
            using (BsonWriter writer = new BsonWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, logEvent);
            }
            var logString = Convert.ToBase64String(ms.ToArray());
            if (_logStorageService == null)
            {
                Console.WriteLine(logString);
                return;
            }
            _logStorageService.Save(logString);
            */
            //TODO
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
