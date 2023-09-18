using System;
using System.Text;

namespace Siesa.SDK.Shared.DTOS
{
    /// <summary>
    /// Representa una acción de suscripción a una cola.
    /// </summary>
    public class QueueSubscribeActionDTO
    {

        //Este target representa la clase donde esta el metodo a ejecutar
        /// <summary>
        /// Obtiene o establece la clase donde se encuentra el método a invocar en la suscripción.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del método a invocar en la suscripción.
        /// </summary>
        public string MethodName { get; set; }
    }
}