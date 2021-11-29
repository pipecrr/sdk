using AuditAppGrpcClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Logs.DataChangeLog
{
    public static class LogService
    {
        public static void SaveDataEntityLog(List<DataEntityLog> logs)
        {
            var dataChangeLog = new SDKGrpcChangeLogStorageService("https://localhost:7168");
            //var tasks = new List<Task>();
            Parallel.ForEach(logs, log => ConvertAndSave(log, dataChangeLog));
            //foreach (var log in logs) { 
            //    string result = JsonConvert.SerializeObject(log);
            //    tasks.Add(dataChangeLog.Save(result));
            //}
            //await Task.WhenAll(tasks);
        }

        private static void ConvertAndSave(DataEntityLog log, SDKGrpcChangeLogStorageService dataChangeLog)
        {
            string result = JsonConvert.SerializeObject(log);
            dataChangeLog.Save(result).Wait();
        }

    }
}