using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Services
{

    public class QueueService : IQueueService
    {
        private readonly IBackendRouterService _backendRouterService;
        private readonly IAuthenticationService _authenticationService;

        public Grpc.Core.AsyncDuplexStreamingCall<Siesa.SDK.Protos.OpeningChannelToBackRequest, Siesa.SDK.Protos.QueueMessageDTO> DuplexStreamingCall { get; set; }

        private Dictionary<string, List<Action<QueueMessageDTO>>> _subscriptionsActions = new();
        public QueueService(IBackendRouterService backendRouterService, IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _backendRouterService = backendRouterService;
        }

        private SDKBusinessModel GetBackend()
        {
            return _backendRouterService.GetSDKBusinessModel("BLSDKQueue", _authenticationService);
        }
        public void SendMessage(string exchangeName, string routingKey, QueueMessageDTO message)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(string exchangeName, string bindingKey, Action<QueueMessageDTO> action = null)
        {
            SubscribeAction(exchangeName, bindingKey, action);

            var BLBackend = GetBackend();

            _ = BLBackend.Call("SubscribeToQueueFront", exchangeName, bindingKey);

            _ = OpenChannel( exchangeName,  bindingKey);

        }

        private async Task OpenChannel(string exchangeName, string bindingKey)
        {
            if (DuplexStreamingCall == null)
            {
                DuplexStreamingCall = await  _backendRouterService.OpenChannelFrontToBack(ExcuteActions);
            }
           await  DuplexStreamingCall.RequestStream.WriteAsync(new OpeningChannelToBackRequest() { ExchangeName = exchangeName, BindingKey = bindingKey });
        }

        private void ExcuteActions(QueueMessageDTO message)
        {
            if (_subscriptionsActions.ContainsKey(message.QueueName))
            {
                var actions = _subscriptionsActions[message.QueueName];

                foreach (var action in actions)
                {
                    action(message);
                }
            }
        }

        public void TestDisconection()
        {
            throw new NotImplementedException();
        }

        private void SubscribeAction(string exchangeName, string bindingKey, Action<QueueMessageDTO> action = null)
        {
            var subscriptionKey = $"{exchangeName}_{bindingKey}";
            
            if (_subscriptionsActions.ContainsKey(subscriptionKey))
            {
                var actions = _subscriptionsActions[subscriptionKey];
                if (actions.Contains(action))
                {
                    return;
                }
                actions.Add(action);
            }
            else
            {
                _subscriptionsActions.Add(subscriptionKey, new List<Action<QueueMessageDTO>> { action });
            }
        }

        public void Unsubscribe(string exchangeName, string bindingKey)
        {
            _subscriptionsActions.Remove($"{exchangeName}_{bindingKey}");
            //_backendRouterService.RemoveChannels($"{exchangeName}_{bindingKey}");
        }
    }
}
