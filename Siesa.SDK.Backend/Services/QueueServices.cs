using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Siesa.SDK.Backend.Services
{
    public class QueueService : BackgroundService
    {

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly Dictionary<string, string> _subscriptions = new Dictionary<string, string>();

        public QueueService()
        {
            var factory = new ConnectionFactory() { HostName = "coder.overjt.com" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        public void Subscribe(string exchangeName, string bindingKey)
        {
            _subscriptions[exchangeName] = bindingKey;
        }
        public void SendMessage(string exchangeName, string routingKey, string message)
        {
            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);

            //Console.WriteLine($"Sent message with routing key '{routingKey}': '{message}'");
        }
        public void StartReceiving()
        {
            if (_subscriptions.Count == 0)
            {
                Console.WriteLine("No subscriptions to start receiving messages.");
                return;
            }
            foreach (var topic in _subscriptions.Keys)
            {
                _channel.ExchangeDeclare(exchange: topic, type: ExchangeType.Topic);

                var queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: queueName, exchange: topic, routingKey: _subscriptions[topic]);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[x] Received '{message}'");
                };
                _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            StartReceiving();

            return Task.CompletedTask;
        }
    }
}
