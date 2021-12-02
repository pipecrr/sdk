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
        public DbSet<S004_User> S004_Users { get; set; }
    }
}
