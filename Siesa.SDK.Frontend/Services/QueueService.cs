using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Core;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Services
{
    /// <summary>
    /// Implementación de un servicio para interactuar con colas de mensajes RabbitMQ.
    /// </summary>
    public class QueueService : IQueueService
    {
        private readonly IBackendRouterService _backendRouterService;
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Canal de comunicación con el Backend.
        /// </summary>
        public Grpc.Core.AsyncDuplexStreamingCall<Siesa.SDK.Protos.OpeningChannelToBackRequest, Siesa.SDK.Protos.QueueMessageDTO> DuplexStreamingCall { get; set; }

        private readonly Dictionary<string, List<Action<QueueMessageDTO>>> _subscriptionsActions = new();

        /// <summary>
        /// Constructor de la clase <see cref="QueueService"/>.
        /// </summary>
        /// <param name="backendRouterService">Servicio de enrutamiento hacia el Backend.</param>
        /// <param name="authenticationService">Servicio de autenticación.</param>
        public QueueService(IBackendRouterService backendRouterService, IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _backendRouterService = backendRouterService;
        }

        private SDKBusinessModel GetBackend()
        {
            return _backendRouterService.GetSDKBusinessModel("BLSDKQueue", _authenticationService);
        }
        /// <summary>
        /// Envía un mensaje a un exchange específico.
        /// </summary>
        /// <param name="exchangeName">Nombre del exchange de destino.</param>
        /// <param name="routingKey">Clave de enrutamiento del mensaje.</param>
        /// <param name="message">Mensaje a enviar.</param>
        public void SendMessage(string exchangeName, string routingKey, QueueMessageDTO message)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Suscribe un método para recibir mensajes desde una cola específica en el Frontend.
        /// </summary>
        /// <param name="exchangeName">Nombre del exchange asociado a la cola.</param>
        /// <param name="bindingKey">Clave de enrutamiento de la cola.</param>
        /// <param name="action">Acción a ejecutar al recibir un mensaje (opcional).</param>
        public void Subscribe(string exchangeName, string bindingKey, Action<QueueMessageDTO> action = null)
        {
            SubscribeAction(exchangeName, bindingKey, action);

            var BLBackend = GetBackend();

            _ = BLBackend.Call("SubscribeToQueueFront", exchangeName, bindingKey);

            _ = OpenChannel(exchangeName, bindingKey);

        }

        private async Task OpenChannel(string exchangeName, string bindingKey)
        {
            if (DuplexStreamingCall == null)
            {
                DuplexStreamingCall = await _backendRouterService.OpenChannelFrontToBack(ExcuteActions).ConfigureAwait(true);
            }
            await DuplexStreamingCall.RequestStream.WriteAsync(new OpeningChannelToBackRequest() { ExchangeName = exchangeName, BindingKey = bindingKey }).ConfigureAwait(true);
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

        /// <summary>
        /// Cancela la suscripción a una cola específica.
        /// </summary>
        /// <param name="exchangeName">Nombre del exchange asociado a la cola.</param>
        /// <param name="bindingKey">Clave de enrutamiento de la cola.</param>
        public void Unsubscribe(string exchangeName, string bindingKey)
        {
            _subscriptionsActions.Remove($"{exchangeName}_{bindingKey}");
            DuplexStreamingCall.Dispose();
        }
    }
}
