using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Backend.Access
{

    public class SDKEntitiesContext : SDKContext
    {
        public SDKEntitiesContext(DbContextOptions options) : base(options)
        {
            
        }
                
    }
}
