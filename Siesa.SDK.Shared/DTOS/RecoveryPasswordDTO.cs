using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Entities;
namespace Siesa.SDK.Backend.Access
{
    public class RecoveryPasswordDTO
    {
        public E00510_ExternalUser PortalUser { get; set; }
        public E00220_User InternalUser { get; set; }
    }
}