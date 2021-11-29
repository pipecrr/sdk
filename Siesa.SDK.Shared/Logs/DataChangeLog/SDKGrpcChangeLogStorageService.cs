﻿
using AuditAppGrpc;
using AuditAppGrpcClient;
using Grpc.Net.Client;
using System;
using static AuditAppGrpc.DataLogChange;

namespace Siesa.SDK.Shared.Logs.DataChangeLog
{
    public class SDKGrpcChangeLogStorageService : ISDKLogStorageService
    {
        private readonly DataLogChangeClient _client;
        private readonly GrpcChannel _channel;
        private bool _writeInConsole;

        public SDKGrpcChangeLogStorageService(string url)
        {
            var _channel = GrpcChannel.ForAddress(url);
            _client = new DataLogChangeClient(_channel);
        }

        public void Save(string json)
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
