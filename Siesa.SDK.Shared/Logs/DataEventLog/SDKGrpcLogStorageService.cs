
using AuditAppGrpc;
using AuditAppGrpcClient;
using Grpc.Net.Client;
using System;
using static AuditAppGrpc.DataLogEvent;

namespace Siesa.SDK.Shared.Logs.DataEventLog
{
    public class SDKGrpcLogStorageService : ISDKLogStorageService
    {
        private readonly DataLogEventClient _client;
        private readonly GrpcChannel _channel;
        private bool _writeInConsole;

        public SDKGrpcLogStorageService(string url)
        {
            var _channel = GrpcChannel.ForAddress(url);
            _client = new DataLogEventClient(_channel);
        }

        public void Save(string json)
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
                var respuesta = _client.StoreLog(request);
                return;
            }
            catch (Grpc.Core.RpcException e)
            {
#if DEBUG
                _writeInConsole = true;
                Console.WriteLine("¡Warning! The log Grpc Service is unavaible");
                Save(json);
                return;
#endif
                throw e;
            }
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
