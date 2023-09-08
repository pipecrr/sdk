
using System.Collections.Generic;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Shared.DTOS
{
    /// <summary>
    /// Representa la configuración de conexión
    /// </summary>
    public class SDKConnectionConfig
    {
        /// <summary>
        /// Obtiene o establece un valor que indica si la conexión es remota.
        /// </summary>
        public bool IsRemote { get; set; }

        /// <summary>
        /// Obtiene o establece la URL de la API a la que se va a conectar.
        /// </summary>
        public string ApiUrl { get; set; }

        /// <summary>
        /// Obtiene o establece el token de autenticación para la conexión.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Obtiene o establece los datos del usuario autenticado.
        /// </summary>
        public JwtUserData User { get; set; }

        /// <summary>
        /// Obtiene o establece la URL de Redis para la gestión de caché
        /// </summary>
        public string RedisUrl { get; set; }
    }

}