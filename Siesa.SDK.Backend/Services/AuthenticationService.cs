using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Criptography;
using System;
using System.Threading.Tasks;
using System.Linq;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.DTOS;

namespace Siesa.SDK.Backend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public string UserToken { get; set; } = "";
        private string _secretKey;
        private int _minutesExp;
        public short RowidCultureChanged { get; set; } = 0;
        private ISDKJWT _sdkJWT;
        public AuthenticationService(ISDKJWT sdkJWT)
        {
            _sdkJWT = sdkJWT;
        }

        private JwtUserData? _user;

        public JwtUserData User { get {
            if(UserToken == ""){
                return null;
            }
            if(_user == null){
                try
                {
                     _user = _sdkJWT.Validate<JwtUserData>(UserToken);

                }catch (System.Exception)
                {
                        
                     _user = null;
                }
              
            }
            return _user;
        }}

        /// <summary>
        /// Portal user. This property represents the portal user associated with the session.
        /// </summary>
        public PortalUserJwt PortalUser {
            get {
                return User?.PortalUser;
            }
        }

        public async Task SetToken(string token, bool saveLocalStorage = true)
        {
            UserToken = token;
        }

        public Task Initialize()
        {
            throw new NotImplementedException();
        }

        public Task Login(string username, string password, short rowidDbConnection, 
            bool isUpdateSession = false,short rowIdCompanyGroup = 1)
        {
            throw new NotImplementedException();
        }

        public Task LoginPortal(string username, string password, short rowidDbConnection, bool isUpdateSession = false, short rowidCompanyGroup = 1)
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }
        public Task LogoutPortal()
        {
            throw new NotImplementedException();
        }
        public Task SetCustomRowidCulture(short rowid)
        {
            throw new NotImplementedException();
        }

        public Task RemoveCustomRowidCulture()
        {
            throw new NotImplementedException();
        }

        [Obsolete("Use GetRowidCulture() instead")]
        public short GetRoiwdCulture()
        {
            return GetRowidCulture();
        }

        public short GetRowidCulture()
        {
            if (User != null)
            {
                return User.RowidCulture;
            }
            else
            {
                return 0;
            }
        }

        public Task SetRowidCompanyGroup(short rowid)
        {
            throw new NotImplementedException();
        }

        public short GetRowidCompanyGroup()
        {
            throw new NotImplementedException();
        }

        public Task SetSelectedConnection(SDKDbConnection selectedConnection)
        {
            throw new NotImplementedException();
        }

        public SDKDbConnection GetSelectedConnection()
        {
            throw new NotImplementedException();
        }

        public string GetConnectionLogo()
        {
            throw new NotImplementedException();
        }

        public string GetConnectionStyle()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsValidToken()
        {
            var user = _sdkJWT.Validate<JwtUserData>(UserToken);
            return user != null;
            // var user = new SDKJWT(_secretKey, _minutesExp).Validate(UserToken);
            // return user != null;
        }

        /// <summary>
        /// Method to send an email to the user with the password recovery link.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="isPortal"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ForgotPasswordAsync(string email, bool isPortal = false){
             throw new NotImplementedException();
        }


        /// <summary>
        /// Method to validate the user token for password recovery.
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="isPortal"></param>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> ValidateUserToken(string userToken, bool isPortal){
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to change the user password.
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="rowIdDBConnection"></param>
        /// <param name="NewPassword"></param>
        /// <param name="ConfirmPassword"></param>
        /// <param name="isPortal"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> ChangePassword(string userToken,short rowIdDBConnection, string NewPassword, string ConfirmPassword,bool isPortal = false)
        {
             throw new NotImplementedException();
        }

        public int GetSelectedSuite()
        {
            throw new NotImplementedException();
        }

        public void SetSelectedSuite(int rowid)
        {
            throw new NotImplementedException();
        }
        public async Task RenewToken()
        {
            throw new NotImplementedException();
        }
        
        public string GetUserPhoto()
        {
            throw new NotImplementedException();
        }

        public async Task SetPreferencesUser(UserPreferencesDTO preferences)
        {
            throw new NotImplementedException();
        }
        public UserPreferencesDTO GetPreferencesUser()
        {
            throw new NotImplementedException();
        }

        public async Task SetUserPhoto(string _data, bool saveLocalStorage = true){
            throw new NotImplementedException();
        }

        public string GetThemeStyle()
        {
            throw new NotImplementedException();
        }

        public Task<string> LoginSessionByToken(string userAccesstoken, short rowidDBConnection)
        {
            throw new NotImplementedException();
        }
    }
}