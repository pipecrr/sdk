using AuditAppGrpcClient;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Siesa.SDK.Shared.Logs.DataChangeLog
{
    public static class LogService
    {
        public static void SaveDataEntityLog(List<DataEntityLog> logs)
        {
            var dataChangeLog = new SDKGrpcChangeLogStorageService("https://localhost:7168");
            foreach (var log in logs) { 
                string result = JsonConvert.SerializeObject(log);
                dataChangeLog.Save(result);
            }
        }

    }
}