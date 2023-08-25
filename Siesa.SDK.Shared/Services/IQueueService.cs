
using System;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Protos;

namespace Siesa.SDK.Shared.Services
{
    public interface IQueueService
    {
        void Subscribe(string exchangeName, string bindingKey, Action<QueueMessageDTO> action = null);
        void SendMessage(string exchangeName, string routingKey, QueueMessageDTO message);

        void TestDisconection();
    }

}