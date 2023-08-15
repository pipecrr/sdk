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
    public interface IQueueService
    {
        void Subscribe(string exchangeName, string bindingKey);
        void SendMessage(string exchangeName, string routingKey, string message);
    }
    public class QueueService : BackgroundService, IQueueService
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

            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: bindingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Received '{message}'");
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }
        public void SendMessage(string exchangeName, string routingKey, string message)
        {
            _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

            var body = Encoding.UTF8.GetBytes(message);
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
    }
}
