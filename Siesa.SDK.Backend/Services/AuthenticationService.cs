using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Criptography;
using System;
using System.Threading.Tasks;
using System.Linq;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Backend.Criptography;

namespace Siesa.SDK.Backend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public string UserToken { get; set; } = "";
        private string _secretKey;
        private int _minutesExp;
        public short RowidCultureChanged { get; set; } = 0;
        private ISDKJWT _sdkJWT;
        public AuthenticationService(ISDKJWT sdkJWT){
            _minutesExp = 120; //TODO: get from config
            _secretKey = "testsecretKeyabc$"; //TODO: get from config
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

        public async Task SetToken(string token, bool saveLocalStorage = true)
        {
            UserToken = token;
        }

        public Task Initialize()
        {
            throw new NotImplementedException();
        }

        public Task Login(string username, string password, short rowIdDBConnection, 
            bool IsUpdateSession = false,short rowIdCompanyGroup = 1)
        {
            throw new NotImplementedException();
        }

        public Task Logout()
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

        public short GetRoiwdCulture()
        {
            throw new NotImplementedException();
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
        public async Task<bool> ForgotPasswordAsync(string email){
             throw new NotImplementedException();
        }
        public async Task<bool> ValidateUserToken(int rowidUser){
            throw new NotImplementedException();
        }
        public async Task<bool> ChangePassword(int rowidUser, string NewPassword="", string ConfirmPassword=""){
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
    }
}