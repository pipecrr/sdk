using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Criptography;
using System;
using System.Threading.Tasks;
using System.Linq;
using Siesa.SDK.Shared.Services;

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

        public void SetToken(string token)
        {
            UserToken = token;
        }

        public Task Initialize()
        {
            throw new NotImplementedException();
        }

        public Task Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }
    }
}