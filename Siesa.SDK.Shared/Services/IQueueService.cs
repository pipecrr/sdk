using System;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Protos;

namespace Siesa.SDK.Shared.Services
{
    /// <summary>
    /// Implementación de un servicio para interactuar con colas de mensajes RabbitMQ.
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// Suscribe un método para recibir mensajes desde una cola específica.
        /// </summary>
        /// <param name="exchangeName">Nombre del exchange asociado a la cola.</param>
        /// <param name="bindingKey">Clave de enrutamiento de la cola.</param>
        /// <param name="action">Acción a ejecutar al recibir un mensaje (opcional).</param>
        void Subscribe(string exchangeName, string bindingKey, Action<QueueMessageDTO> action = null);

        /// <summary>
        /// Envía un mensaje a un exchange específico.
        /// </summary>
        /// <param name="exchangeName">Nombre del exchange de destino.</param>
        /// <param name="routingKey">Clave de enrutamiento del mensaje.</param>
        /// <param name="message">Mensaje a enviar.</param>
        void SendMessage(string exchangeName, string routingKey, QueueMessageDTO message);

        /// <summary>
        /// Anula la suscripción a una cola específica.
        /// </summary>
        /// <param name="exchangeName">Nombre del exchange asociado a la cola.</param>
        /// <param name="bindingKey">Clave de enrutamiento de la cola.</param>
        void Unsubscribe(string exchangeName, string bindingKey);
    }
}
