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

        public AuthenticationService(){
            _minutesExp = 120; //TODO: get from config
            _secretKey = "testsecretKeyabc$"; //TODO: get from config
        }

        private JwtUserData? _user;

        public JwtUserData User { get {
            if(UserToken == ""){
                return null;
            }
            if(_user == null){
                _user = new SDKJWT(_secretKey, _minutesExp).Validate(UserToken);
            }
            return _user;
        }}

        public async Task SetToken(string token)
        {
            UserToken = token;
        }

        public Task Initialize()
        {
            throw new NotImplementedException();
        }

        public Task Login(string username, string password, short rowIdDBConnection)
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
            var user = new SDKJWT(_secretKey, _minutesExp).Validate(UserToken);
            return user != null;
        }
        public async Task<bool> ForgotPasswordAsync(string email){
             throw new NotImplementedException();
        }
        public async Task<bool> ValidateUserForChangedPassword(int rowidUser, string NewPassword="", string ConfirmPassword=""){
             throw new NotImplementedException();
        }

        
    }
}