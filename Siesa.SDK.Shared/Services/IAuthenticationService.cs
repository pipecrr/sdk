using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Services
{
    public class JwtUserData {
        public int Rowid { get; set; }
        public string Path { get; set; }
        public string PasswordRecoveryEmail { get; set; }
        // public string Name { get; set; }

        // public string[] Roles { get; set; }
        // public string[] Teams { get; set; }

        // public bool IsSuperAdmin { get; set; }

        public override string ToString()
        {
            return $"{Path}";
        }
    }
    
    public interface IAuthenticationService
    {
        string UserToken { get; }
        JwtUserData User { get; }
        Task Initialize();
        Task Login(string username, string password);
        Task Logout();
        public void SetToken(string token);
    }

    
}