using System.Collections.Generic;
using System.Threading.Tasks;
using Siesa.SDK.Shared.DTOS;

namespace Siesa.SDK.Shared.Services
{
    /// <summary>
    /// Represents a session role with its rowid and name.
    /// </summary>
    public class SessionRol
    {
        /// <summary>
        /// Gets or sets the rowid of the session role.
        /// </summary>
        public int Rowid { get; set; }

        /// <summary>
        /// Gets or sets the name of the session role.
        /// </summary>
        public string Name { get; set; }
    }
    /// <summary>
    /// Represents the data stored in a JSON Web Token (JWT) for a user's session.
    /// </summary>
    public class JwtUserData
    {
        /// <summary>
        /// Gets or sets the rowid of the user.
        /// </summary>
        public int Rowid { get; set; }

        /// <summary>
        /// Gets or sets the path of the user.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the email used for password recovery.
        /// </summary>
        public string PasswordRecoveryEmail { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the description of the user.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the rowid of the user's culture.
        /// </summary>
        public short RowidCulture { get; set; }

        /// <summary>
        /// Gets or sets the list of session roles assigned to the user.
        /// </summary>
        public List<SessionRol> Roles { get; set; } = new List<SessionRol>();

        /// <summary>
        /// Gets or sets the rowid of the user's company group.
        /// </summary>
        public short RowidCompanyGroup { get; set; }

        /// <summary>
        /// Gets or sets the rowid of the database connection for the user's session.
        /// </summary>
        public short RowIdDBConnection { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of feature permissions for the user.
        /// </summary>
        public Dictionary<string, List<int>> FeaturePermissions { get; set; } = new Dictionary<string, List<int>>();

        /// <summary>
        /// Gets or sets a value indicating whether the user is an administrator.
        /// </summary>
        public bool IsAdministrator { get; set; }

        /// <summary>
        /// Gets or sets the rowid of the user's profile picture attachment.
        /// </summary>
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
    
    /// <summary>
    /// Represents the authentication service interface for user login, logout, and related operations.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Gets the user token.
        /// </summary>
        string UserToken { get; }
        /// <summary>
        /// Gets the JwtUserData.
        /// </summary>
        JwtUserData User { get; }
        /// <summary>
        /// Gets the  PortalUserJwt
        /// </summary>
        PortalUserJwt PortalUser { get; }
        /// <summary>
        /// Gets the rowid of the culture changed.
        /// </summary>
        short RowidCultureChanged { get; set; }

        /// <summary>
        /// Initializes the authentication service.
        /// </summary>
        Task Initialize();

        /// <summary>
        /// Logs in the user with the provided credentials.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="rowidDbConnection">The identifier of the database connection.</param>
        /// <param name="isUpdateSession">Indicates if the session should be updated.</param>
        /// <param name="rowIdCompanyGroup">The identifier of the company group.</param>
        Task Login(string username, string password, short rowidDbConnection,
                     bool isUpdateSession = false, short rowIdCompanyGroup = 1);

        /// <summary>
        /// Logs in the portal user with the provided credentials.
        /// </summary>
        /// <param name="username">The username of the portal user.</param>
        /// <param name="password">The password of the portal user.</param>
        /// <param name="rowidDbConnection">The identifier of the database connection.</param>
        /// <param name="isUpdateSession">Indicates if the session should be updated.</param>
        /// <param name="rowidCompanyGroup">The identifier of the company group.</param>
        Task LoginPortal(string username, string password, short rowidDbConnection, bool isUpdateSession = false, short rowidCompanyGroup = 1);

        /// <summary>
        /// Logs out the user.
        /// </summary>
        Task Logout();

        /// <summary>
        /// Logs out the portal user.
        /// </summary>
        Task LogoutPortal();

        /// <summary>
        /// Sets the authentication token.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="saveLocalStorage">Indicates if the token should be saved in local storage.</param>
        public Task SetToken(string token, bool saveLocalStorage = true);
        Task SetCustomRowidCulture(short rowid);

        /// <summary>
        /// Gets the custom rowid for the culture changed.
        /// </summary>
        /// <returns>The custom rowid for the culture changed.</returns>

        short GetRoiwdCulture();
        /// <summary>
        /// Sets the rowid for the selected company group.
        /// </summary>
        /// <param name="rowid">The rowid for the selected company group.</param>
        Task SetRowidCompanyGroup(short rowid);

        /// <summary>
        /// Removes the custom rowid for the culture changed.
        /// </summary>
        Task RemoveCustomRowidCulture();


        /// <summary>
        /// Gets the rowid for the selected company group.
        /// </summary>
        /// <returns>The rowid for the selected company group.</returns>
        short GetRowidCompanyGroup();

        /// <summary>
        /// Sets the selected database connection.
        /// </summary>
        /// <param name="selectedConnection">The selected database connection.</param>
        Task SetSelectedConnection(SDKDbConnection selectedConnection);

        /// <summary>
        /// Gets the selected database connection information.
        /// </summary>
        /// <returns>The <see cref="SDKDbConnection"/> representing the selected database connection.</returns>
        SDKDbConnection GetSelectedConnection();

        /// <summary>
        /// Gets the identifier of the selected suite.
        /// </summary>
        /// <returns>The identifier of the selected suite.</returns>
        int GetSelectedSuite();

        /// <summary>
        /// Sets the identifier of the selected suite.
        /// </summary>
        /// <param name="rowid">The identifier of the selected suite.</param>
        void SetSelectedSuite(int rowid);

        /// <summary>
        /// Gets the logo associated with the database connection.
        /// </summary>
        /// <returns>The logo associated with the database connection.</returns>
        string GetConnectionLogo();

        /// <summary>
        /// Gets the style associated with the database connection.
        /// </summary>
        /// <returns>The style associated with the database connection.</returns>
        string GetConnectionStyle();
        /// <summary>
        /// Checks if the current authentication token is valid.
        /// </summary>
        /// <returns>A boolean value indicating whether the authentication token is valid.</returns>
        Task<bool> IsValidToken();

        /// <summary>
        /// Method to send an email to the user with the password recovery link.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="isPortal">Indicates if the password recovery is for a portal user.</param>
        void ForgotPasswordAsync(string email, bool isPortal = false);

        /// <summary>
        /// Method to validate the user token for password recovery.
        /// </summary>
        /// <param name="userToken">The user token for password recovery.</param>
        /// <param name="isPortal">Indicates if the password recovery is for a portal user.</param>
        /// <returns>A boolean value indicating whether the user token is valid for password recovery.</returns>
        Task<bool> ValidateUserToken(string userToken, bool isPortal);

        /// <summary>
        /// Changes the password of a user with the specified recovery token.
        /// </summary>
        /// <param name="userToken">Recovery token of the user.</param>
        /// <param name="rowIdDBConnection">Identifier of the database connection.</param>
        /// <param name="NewPassword">New password to be assigned to the user.</param>
        /// <param name="ConfirmPassword">Confirmation of the new password.</param>
        /// <param name="isPortal">Indicates if the password change is for a portal user.</param>
        /// <returns>A boolean value indicating whether the password change was successful.</returns>
        Task<bool> ChangePassword(string userToken, short rowIdDBConnection, string NewPassword, string ConfirmPassword, bool isPortal = false);
        /// <summary>
        /// Renews the authentication token for the current user.
        /// </summary>
        Task RenewToken();
        /// <summary>
        /// Gets the user's photo URL.
        /// </summary>
        /// <returns>The URL of the user's photo.</returns>
        string GetUserPhoto();
        /// <summary>
        /// Sets the user's photo data.
        /// </summary>
        /// <param name="data">The photo data to set.</param>
        /// <param name="saveLocalStorage">Optional. Determines whether to save the photo data in local storage. Default is true.</param>

        Task SetUserPhoto(string data, bool saveLocalStorage = true);

        /// <summary>
        /// Sets the user's preferences.
        /// </summary>
        /// <param name="preferences">The <see cref="UserPreferencesDTO"/> object representing the user's preferences.</param>
        Task SetPreferencesUser(UserPreferencesDTO preferences);

        /// <summary>
        /// Gets the user's preferences.
        /// </summary>
        /// <returns>The <see cref="UserPreferencesDTO"/> object representing the user's preferences.</returns>
        UserPreferencesDTO GetPreferencesUser();

        /// <summary>
        /// Gets the theme style for the current user.
        /// </summary>
        /// <returns>The theme style.</returns>
        string GetThemeStyle();

        /// <summary>
        /// Logs in the user using the specified user access token and database connection.
        /// </summary>
        /// <param name="userAccesstoken">The user access token to use for login.</param>
        /// <param name="rowidDBConnection">The rowid of the database connection to use for login.</param>
        /// <returns>The authentication token as a string.</returns>
        Task<string> LoginSessionByToken(string userAccesstoken, short rowidDBConnection);
    }
}