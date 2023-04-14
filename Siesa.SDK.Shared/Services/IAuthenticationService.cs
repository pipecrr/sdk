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

        public Dictionary<string, List<int>> FeaturePermissions { get; set; } = new();
        // public string[] Teams { get; set; }

        public bool IsAdministrator { get; set; }
        public int? RowidAttachmentUserProfilePicture { get; set; }
        
        public override string ToString()
        {
            return $"({Id}) - {Name}";
        }
    }
    
    public interface IAuthenticationService
    {
        string UserToken { get; }
        JwtUserData User { get; }
        short RowidCultureChanged { get; set; }
        Task Initialize();
        Task Login(string username, string password, short rowIdDBConnection,
                     bool IsUpdateSession = false, short rowIdCompanyGroup = 1);
        Task Logout();
        public Task SetToken(string token, bool saveLocalStorage = true);
        Task SetCustomRowidCulture(short rowid);
        short GetRoiwdCulture();
        Task SetRowidCompanyGroup(short rowid);
        Task RemoveCustomRowidCulture();

        short GetRowidCompanyGroup();
        Task SetSelectedConnection(SDKDbConnection selectedConnection);
        SDKDbConnection GetSelectedConnection();
        int GetSelectedSuite();
        void SetSelectedSuite(int rowid);
        string GetConnectionLogo();
        string GetConnectionStyle();
        Task<bool> IsValidToken();
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ValidateUserToken(int rowidUser);
        Task<bool> ChangePassword(int rowidUser, string NewPassword, string ConfirmPassword);
        Task RenewToken();
        
    }

    
}