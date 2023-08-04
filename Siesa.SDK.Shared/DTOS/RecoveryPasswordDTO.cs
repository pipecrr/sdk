using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Entities;
namespace Siesa.SDK.Backend.Access
{

    /// <summary>
    /// DTO for managing internal and external user password recovery
    /// </summary>
    public class RecoveryPasswordDTO
    {
        /// <summary>
        /// External user, Represents portal users
        /// </summary>
        /// 
        public E00510_ExternalUser ExternalUser { get; set; }
        

        /// <summary>
        /// Internal user, Represents usert of main application
        /// </summary>
        public E00220_User InternalUser { get; set; }
    }
}