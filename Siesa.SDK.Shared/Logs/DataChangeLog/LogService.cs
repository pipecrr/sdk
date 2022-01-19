﻿using AuditAppGrpcClient;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Logs.DataChangeLog
{
    public static class LogService
    {
        public static void SaveDataEntityLog(List<DataEntityLog> logs)
        {
            var dataChangeLog = new SDKGrpcChangeLogStorageService("http://172.16.1.88:5000");
            Parallel.ForEach(logs, log => ConvertAndSave(log, dataChangeLog));
        }

        private static void ConvertAndSave(DataEntityLog log, SDKGrpcChangeLogStorageService dataChangeLog)
        {
            string result = JsonConvert.SerializeObject(log);
            dataChangeLog.Save(result).Wait();
        }

    }
}