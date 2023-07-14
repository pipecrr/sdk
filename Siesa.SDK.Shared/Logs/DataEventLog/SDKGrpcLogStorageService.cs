
using AuditAppGrpcClient;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Siesa.SDK.Protos;
using System;
using System.Threading.Tasks;
using static Siesa.SDK.Protos.DataLogEvent;
using Siesa.SDK.Shared.GRPCServices;

namespace Siesa.SDK.Shared.Logs.DataEventLog
{
    public class SDKGrpcLogStorageService : ISDKLogStorageService
    {
        private readonly DataLogEventClient _client;
        private readonly GrpcChannel _channel;
        private bool _writeInConsole;

        public SDKGrpcLogStorageService(IConfiguration configuration)
        {
            var auditUrl = configuration["ServiceConfiguration:AuditServerUrl"];
            _channel = GrpcUtils.GetChannel(auditUrl);
            _client = new DataLogEventClient(_channel);
        }

        public async Task Save(string json)
        {
            // TODO - Async
            if (_writeInConsole)
            {
                Console.WriteLine(json);
                return;
            }

            var request = new StoreEventRequest
            {
                Json = json
            };
            try
            {
                var respuesta = await _client.StoreLogAsync(request);
                return;
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

        public QueryLogReply QueryEntityLog(QueryLogRequest request)
        {
            try
            {
                return  _client.QueryLog(request);
            }
            catch (Grpc.Core.RpcException)
            {
                
            }
            return null;
        }

        ~SDKGrpcLogStorageService()
        {
            if (_channel == null)
            {
                return;
            }
            _channel.ShutdownAsync().Wait();
        }
    }
}
