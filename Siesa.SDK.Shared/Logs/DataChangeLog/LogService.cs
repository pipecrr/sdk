using AuditAppGrpcClient;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Siesa.SDK.Protos;
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

        public static QueryLogReply QueryEntityLog(QueryLogRequest request, System.IServiceProvider serviceProvider)
        {
            var dataChangeLog =  ActivatorUtilities.CreateInstance<SDKGrpcChangeLogStorageService>(serviceProvider);
            return dataChangeLog.QueryEntityLog(request);
        }

        private static void ConvertAndSave(DataEntityLog log, SDKGrpcChangeLogStorageService dataChangeLog)
        {
            string result = JsonConvert.SerializeObject(log);
            dataChangeLog.Save(result).Wait();
        }

    }
}