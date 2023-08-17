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

namespace Siesa.SDK.Backend.Services
{
    public interface IQueueService
    {
        void Subscribe(string exchangeName, string bindingKey, Action<string> action = null);
        void SendMessage(string exchangeName, string routingKey, string message);

        void TestDisconection();
    }

    public class QueueService : BackgroundService, IQueueService
    {

        private IConnection _connection;
        private IModel _channel;
        private Dictionary<string, string> _subscriptions = new Dictionary<string, string>();
        private Dictionary<string, List<QueueSubscribeActionDTO>> _subscriptionsActions = new();
        private int _reconectAttemp = 0;
        private ConnectionFactory factory = new ConnectionFactory() { HostName = "coder.overjt.com" };
        private readonly IServiceProvider _serviceProvider;

        public QueueService()
        {
            //_connection = factory.CreateConnection();
            //_channel = _connection.CreateModel();
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

        public void TestDisconection()
        {
            _connection.Close();
        }
        public void Subscribe(string exchangeName, string bindingKey, Action<string> action = null)
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

                if (_subscriptionsActions.ContainsKey($"{exchange}_{routingKey}"))
                {
                    InvokeAction(exchange, routingKey, receivedQueueMessage);
                }
                Console.WriteLine($"[x] Received Message: '{receivedQueueMessage.Message}', Rowid: '{receivedQueueMessage.Rowid}'");
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
        public void SendMessage(string exchangeName, string routingKey, QueueMessageDTO message)
        {
            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);


            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);

            //Console.WriteLine($"Sent message with routing key '{routingKey}': '{message}'");
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(200);
            }
            // return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        private void SubscribeAction(string exchangeName, string bindingKey, Action<string> action = null)
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
                        //TODO: Check if method is async - QueueMessageDTO
                        method.Invoke(blInstance, new object[] { message });
                    }
                }
                catch (System.Exception)
                {
                }
            }
        }
    }
}
