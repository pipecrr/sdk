using System.Collections.Generic;
using System.Threading.Tasks;
using Siesa.SDK.Shared.DTOS;

namespace Siesa.SDK.Shared.Services
{
    public class SessionRol {
        public int Rowid { get; set; }
        public string Name { get; set; }
    }
    public class JwtUserData {
        public int Rowid { get; set; }
        public string Path { get; set; }
        public string PasswordRecoveryEmail { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public short RowidCulture { get; set; }

        public List<SessionRol> Roles { get; set; } = new List<SessionRol>();
        
        public short RowidCompanyGroup { get; set; } 
        public short RowIdDBConnection { get; set; }

        public Dictionary<int, Dictionary<int, bool>> FeaturePermissions { get; set; } = new Dictionary<int, Dictionary<int, bool>>();
        // public string[] Teams { get; set; }

        // public bool IsSuperAdmin { get; set; }

        public override string ToString()
        {
            return $"({Id}) - {Name}";
        }
    }
    
    public interface IAuthenticationService
    {
        string UserToken { get; }
        JwtUserData User { get; }
        Task Initialize();
        Task Login(string username, string password, short rowIdDBConnection);
        Task Logout();
        public Task SetToken(string token);
        Task SetCustomRowidCulture(short rowid);
        short GetRoiwdCulture();
        Task SetRowidCompanyGroup(short rowid);
        Task RemoveCustomRowidCulture();

        short GetRowidCompanyGroup();
        Task SetSelectedConnection(SDKDbConnection selectedConnection);
        SDKDbConnection GetSelectedConnection();
        string GetConnectionLogo();
        string GetConnectionStyle();
        Task<bool> IsValidToken();
    }

    
}