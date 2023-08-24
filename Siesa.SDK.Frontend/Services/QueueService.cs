using System;
using System.Collections.Generic;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Services
{

    public class QueueService : IQueueService
    {
        private readonly IBackendRouterService _backendRouterService;
        private readonly IAuthenticationService _authenticationService;

        private Dictionary<string, List<QueueSubscribeActionDTO>> _subscriptionsActions = new();
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

            _ = BLBackend.Call("SubscribeToQueueFront",exchangeName, bindingKey );   

        }

        public void TestDisconection()
        {
            throw new NotImplementedException();
        }

        private void SubscribeAction(string exchangeName, string bindingKey, Action<QueueMessageDTO> action = null)
        {
            var subscriptionKey = $"{exchangeName}_{bindingKey}";
            var methodName = action?.Method.Name ?? null;
            var blTarget = action?.Target?.GetType()?.FullName ?? null;

            
            if (_subscriptionsActions.ContainsKey(subscriptionKey))
            {
                var actions = _subscriptionsActions[subscriptionKey];
                if (actions.Exists(a => a.MethodName == methodName && a.Target == blTarget))
                {
                    return;
                }
                actions.Add(new QueueSubscribeActionDTO { MethodName = methodName, Target = blTarget });
            }
            else
            {
                _subscriptionsActions.Add(subscriptionKey, new List<QueueSubscribeActionDTO> { new QueueSubscribeActionDTO { MethodName = methodName, Target = blTarget } });
            }
        }
    }
}
