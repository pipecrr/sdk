using Siesa.SDK.Entities;
using System.Collections.Generic;

namespace Siesa.SDK.Shared.DTOS
{
    public class SDKAuthenticationMiddlewareDTO
    {
        public E00220_User? User { get; set; }
        public bool CreateUser { get; set; }
        public List<int> DefaultRoles { get; set; }
        public List<int> DefaultGroups { get; set; }
    }
}