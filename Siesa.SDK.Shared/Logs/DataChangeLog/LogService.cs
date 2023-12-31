﻿using AuditAppGrpcClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Logs.DataEventLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace Siesa.SDK.Shared.Logs.DataChangeLog
{
    public static class LogService
    {
        public static void SaveDataEntityLog(List<DataEntityLog> logs, IConfiguration configuration)
        {
            var dataChangeLog =  new SDKGrpcChangeLogStorageService(configuration);
            Parallel.ForEach(logs, log => ConvertAndSave(log, dataChangeLog));
        }

        public static QueryLogReply QueryEntityLog(QueryLogRequest request, IConfiguration configuration)
        {
            var dataChangeLog =  new SDKGrpcChangeLogStorageService(configuration);
            return dataChangeLog.QueryEntityLog(request);
        }

        public static QueryLogReply QueryEntityEventLog(QueryLogRequest request, IConfiguration configuration)
        {
            var dataChangeLog =  new SDKGrpcLogStorageService(configuration);
            return dataChangeLog.QueryEntityLog(request);
        }

        private static void ConvertAndSave(DataEntityLog log, SDKGrpcChangeLogStorageService dataChangeLog)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, log);
            }

            string data = Convert.ToBase64String(ms.ToArray());
            dataChangeLog.Save(data).Wait();
        }

    }
}