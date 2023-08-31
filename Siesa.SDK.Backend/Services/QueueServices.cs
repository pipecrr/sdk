using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.Application;
using Siesa.SDK.Shared.DTOS;
using Newtonsoft.Json;
using System.Reflection;
using System.Runtime.CompilerServices;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Protos;
using Microsoft.Extensions.Configuration;

namespace Siesa.SDK.Backend.Services
{
    /// <summary>
    /// Implementación de un servicio para interactuar con colas de mensajes RabbitMQ.
    /// </summary>
    public class QueueService : BackgroundService, IQueueService
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;
        private readonly Dictionary<string, string> _subscriptions = new Dictionary<string, string>();
        private readonly Dictionary<string, List<QueueSubscribeActionDTO>> _subscriptionsActions = new();
        private int _reconectAttemp = 0;
        private ConnectionFactory factory;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor de la clase <see cref="QueueService"/>.
        /// </summary>
        public QueueService(IConfiguration configuration)
        {
            _configuration = configuration;

            factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQ:HostName"],
            };
            
            Connect();
            _serviceProvider = SDKApp.GetServiceProvider();
        }
        //TODO: Reconnect 
        private void Connect()
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _reconectAttemp = 0;
                Console.WriteLine("Connection to RabbitMQ established.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to RabbitMQ: {ex.Message}");
                Reconnect();
            }
        }
        private void OnConnectionShutdown(object sender, ShutdownEventArgs args)
        {
            Console.WriteLine($"Connection to RabbitMQ was shut down: {_connection.CloseReason}. Trying to reconnect...");
            Reconnect();
        }
        private void Reconnect()
        {
            while (!_connection.IsOpen && _reconectAttemp < 6)
            {
                try
                {
                    _reconectAttemp++;
                    Console.WriteLine($"Reconnecting attempt {_reconectAttemp}");
                    Connect();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during reconnection: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Suscribe un método para recibir mensajes desde una cola específica.
        /// </summary>
        /// <param name="exchangeName">Nombre del exchange asociado a la cola.</param>
        /// <param name="bindingKey">Clave de enrutamiento de la cola.</param>
        /// <param name="action">Acción a ejecutar al recibir un mensaje (opcional).</param>
        public void Subscribe(string exchangeName, string bindingKey, Action<QueueMessageDTO> action = null)
        {
            if (action != null)
            {

                SubscribeAction(exchangeName, bindingKey, action);
            }

            if (_subscriptions.ContainsKey(exchangeName) && _subscriptions[exchangeName] == bindingKey)
            {
                return;
            }

            _subscriptions[exchangeName] = bindingKey;

            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: bindingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var receivedQueueMessage = JsonConvert.DeserializeObject<QueueMessageDTO>(message);

                var routingKey = ea.RoutingKey;
                var exchange = ea.Exchange;
                
                receivedQueueMessage.QueueName = $"{exchange}_{routingKey}";

                if (_subscriptionsActions.ContainsKey($"{exchange}_{routingKey}"))
                {
                    InvokeAction(exchange, routingKey, receivedQueueMessage);
                }
                Console.WriteLine($"[x] Received Message: '{receivedQueueMessage.Message}', Rowid: '{receivedQueueMessage.Rowid}'");
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        /// <summary>
        /// Envía un mensaje a un exchange específico.
        /// </summary>
        /// <param name="exchangeName">Nombre del exchange de destino.</param>
        /// <param name="routingKey">Clave de enrutamiento del mensaje.</param>
        /// <param name="message">Mensaje a enviar.</param>
        public void SendMessage(string exchangeName, string routingKey, QueueMessageDTO message)
        {
            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);


            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
        }

        /// <summary>
        /// Metodo para ejecutar en segundo plano el servicio de colas.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(200);
            }
        }

        /// <summary>
        /// Libera los recursos utilizados por la clase <see cref="QueueService"/>.
        /// </summary>
        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        private void SubscribeAction(string exchangeName, string bindingKey, Action<QueueMessageDTO> action = null)
        {
            var methodName = action?.Method.Name ?? null;
            var blTarget = action?.Target?.GetType()?.FullName ?? null;
            var subscriptionKey = $"{exchangeName}_{bindingKey}";

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
        private void InvokeAction(string exchange, string routingKey, QueueMessageDTO message)
        {
            var actions = _subscriptionsActions[$"{exchange}_{routingKey}"];
            foreach (var action in actions)
            {
                try
                {
                    Type blType = Utilities.SearchType(action.Target, true);
                    if (blType != null)
                    {
                        var blInstance = ActivatorUtilities.CreateInstance(_serviceProvider, blType);
                        var method = blType.GetMethod(action.MethodName);

                        method.Invoke(blInstance, new object[] { message });
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Error invoking action: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// Metodo no implementado en Backend
        /// </summary>
        /// <param name="exchangeName"></param>
        /// <param name="bindingKey"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Unsubscribe(string exchangeName, string bindingKey)
        {
            throw new NotImplementedException();
        }
    }
}
