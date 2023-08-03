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
        
        /// <summary>
        /// Portal user. This property represents the portal user associated with the session.
        /// </summary>
        public PortalUserJwt PortalUser { get; set; }
        /// <summary>
        /// Overrides the ToString() method to provide a custom string representation of the JwtUserData.
        /// </summary>
        /// <returns>A formatted string containing the user id and name.</returns>
        public override string ToString()
        {
            return $"({Id}) - {Name}";
        }
    }
    
    /// <summary>
    /// DTO for portal user
    /// </summary>
    public class PortalUserJwt
    {
        /// <summary>
        /// Rowid. This property represents the row identifier of the user in the external system.
        /// </summary>
        public int Rowid { get; set; }
        /// <summary>
        /// User id. This property represents the unique identifier for the portal user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Main record rowid. This property represents the row identifier of the main record associated with the user.
        /// </summary>
        public long RowidMainRecord { get; set; }

        /// <summary>
        /// Overrides the ToString() method to provide a custom string representation of the PortalUserDTO.
        /// </summary>
        /// <returns>A formatted string containing the user id and name.</returns>
        public override string ToString()
        {
            return $"{Id}";
        }
    }
    
    public interface IAuthenticationService
    {
        string UserToken { get; }
        string PortalUserToken { get; }
        JwtUserData User { get; }
        PortalUserJwt PortalUser { get; }
        short RowidCultureChanged { get; set; }

        Task Initialize();
        Task Login(string username, string password, short rowidDbConnection,
                     bool isUpdateSession = false, short rowIdCompanyGroup = 1);
        Task LoginPortal(string username, string password, short rowidDbConnection, bool isUpdateSession = false, short rowidCompanyGroup = 1);
        Task Logout();
        Task LogoutPortal();
        public Task SetToken(string token, bool saveLocalStorage = true);
        public Task SetTokenPortal(string token, bool saveLocalStorage = true);
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
        void ForgotPasswordAsync(string email);
        Task<bool> ValidateUserToken(string userToken);
        Task<bool> ChangePassword(string userToken,short rowIdDBConnection, string NewPassword, string ConfirmPassword );
        Task RenewToken();
        string GetUserPhoto();
        Task SetUserPhoto(string data, bool saveLocalStorage = true);
        Task SetPreferencesUser(UserPreferencesDTO preferences);
        UserPreferencesDTO GetPreferencesUser();
        string GetThemeStyle();
        Task<string> LoginSessionByToken(string userAccesstoken, short rowidDBConnection);
    }

    
}