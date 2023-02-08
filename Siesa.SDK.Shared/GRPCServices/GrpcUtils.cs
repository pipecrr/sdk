using System;
using System.Net.Http;
using Grpc.Net.Client;

namespace Siesa.SDK.Shared.GRPCServices
{
    public static class GrpcUtils
    {
        public static GrpcChannel GetChannel(string url)
        {
            var httpHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
            {
                HttpHandler = httpHandler,
                MaxReceiveMessageSize = 1 * 1024 * 1024 * 1024
            });
            return channel;
        }
    }


}