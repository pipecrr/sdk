using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Hosting;

namespace Siesa.SDK.Backend.Services
{
    public class QueueService //: BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        public QueueService()
        {
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            var factory = new ConnectionFactory() { HostName = "coder.overjt.com" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic);

            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: _bindingKey);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[x] Received '{message}'");
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            Console.WriteLine($"Waiting for messages with binding key '{_bindingKey}'");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
