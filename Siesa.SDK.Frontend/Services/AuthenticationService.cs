using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Criptography;
using System;
using System.Threading.Tasks;
using System.Linq;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private string _secretKey;
        private int _minutesExp;

        public string UserToken { get; private set; } = "";

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

        public AuthenticationService(
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        ) {
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;

            _minutesExp = 120; //TODO: get from config
            _secretKey = "testsecretKeyabc$"; //TODO: get from config
        }

        public async Task Initialize()
        {
            UserToken = await _localStorageService.GetItemAsync<string>("usertoken");
            Console.WriteLine($"UserToken: {UserToken}");
        }

        public async Task Login(string username, string password, short rowIdDBConnection)
        {
            var BLuser =  Frontend.BusinessManagerFrontend.Instance.GetBusiness("BLUser", this);
            if(BLuser == null)
            {
                throw new Exception("Login Service not found");
            }
            var loginRequest = await BLuser.Call("SignIn", username, password, rowIdDBConnection);
            if (loginRequest.Success)
            {
                UserToken = loginRequest.Data;
                await _localStorageService.SetItemAsync("usertoken", UserToken);
            }else{
                if(loginRequest.Errors != null && loginRequest.Errors.Count > 0)
                {
                    throw new Exception(loginRequest.Errors.FirstOrDefault());
                }else{
                    throw new Exception("Login failed");
                }
            }
        }

        public async Task Logout()
        {
            UserToken = "";
            await _localStorageService.RemoveItemAsync("usertoken");
            _navigationManager.NavigateTo("login");
        }

        public void SetToken(string token)
        {
            UserToken = token;
        }
    }
}