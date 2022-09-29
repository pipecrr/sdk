using AuditAppGrpcClient;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Logs.DataChangeLog
{
    public static class LogService
    {
        public static void SaveDataEntityLog(List<DataEntityLog> logs, System.IServiceProvider serviceProvider)
        {
            var dataChangeLog =  ActivatorUtilities.CreateInstance<SDKGrpcChangeLogStorageService>(serviceProvider);
            Parallel.ForEach(logs, log => ConvertAndSave(log, dataChangeLog));
        }

        private static void ConvertAndSave(DataEntityLog log, SDKGrpcChangeLogStorageService dataChangeLog)
        {
            string result = JsonConvert.SerializeObject(log);
            dataChangeLog.Save(result).Wait();
        }

    }
}