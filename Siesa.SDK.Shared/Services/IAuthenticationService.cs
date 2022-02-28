using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Services
{
    public class JwtUserData {
        public int Rowid { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{UserName} - {Name}";
        }
    }
    
    public interface IAuthenticationService
    {
        string UserToken { get; }
        JwtUserData User { get; }
        Task Initialize();
        Task Login(string username, string password);
        Task Logout();
    }

    
}