
using AuditAppGrpcClient;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Configurations;
using System;
using System.Threading.Tasks;
using static Siesa.SDK.Protos.DataLogChange;

namespace Siesa.SDK.Shared.Logs.DataChangeLog
{
    public class SDKGrpcChangeLogStorageService : ISDKLogStorageService
    {
        private readonly DataLogChangeClient _client;
        private readonly GrpcChannel _channel;
        private bool _writeInConsole;

        public SDKGrpcChangeLogStorageService(IConfiguration configuration)
        {
            var auditUrl = configuration["ServiceConfiguration:AuditServerUrl"];
            var _channel = GrpcChannel.ForAddress(auditUrl);
            _client = new DataLogChangeClient(_channel);     
        }

        public QueryLogReply QueryEntityLog(QueryLogRequest request)
        {
            try
            {
                return  _client.QueryLog(request);
            }
            catch (Grpc.Core.RpcException e)
            {
                
            }
            return null;
        }

        public async Task Save(string json)
        {
            // TODO - Async
            if (_writeInConsole)
            {
                Console.WriteLine(json);
                return;
            }

            var request = new StoreLogRequest
            {
                Json = json
            };
            try
            {
                var respuesta = await _client.StoreLogAsync(request);
            }
            catch (Grpc.Core.RpcException e)
            {
#if DEBUG
            _writeInConsole = true;
            Console.WriteLine("¡Warning! The log Grpc Service is unavaible");
            Save(json).Wait();
            return;
#endif
                throw e;
            }
        }

        ~SDKGrpcChangeLogStorageService()
        {
            if (_channel == null)
            {
                return;
            }
            _channel.ShutdownAsync().Wait();
        }
    }
}
