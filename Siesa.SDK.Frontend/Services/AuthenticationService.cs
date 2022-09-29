using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Criptography;
using System;
using System.Threading.Tasks;
using System.Linq;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.DTOS;
using Newtonsoft.Json;

namespace Siesa.SDK.Frontend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private IBackendRouterService _backendRouterService;
        private string _secretKey;
        private int _minutesExp;

        private short CustomRowidCulture = 0;

        private SDKDbConnection SelectedConnection = new SDKDbConnection();

        public string UserToken { get; private set; } = "";

        private JwtUserData? _user;
        public JwtUserData User
        {
            get
            {
                if (UserToken == "")
                {
                    return null;
                }
                if (_user == null)
                {
                    _user = new SDKJWT(_secretKey, _minutesExp).Validate(UserToken);
                }
                return _user;
            }
        }

        public AuthenticationService(
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            IBackendRouterService BackendRouterService
        )
        {
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _backendRouterService = BackendRouterService;

            _minutesExp = 120; //TODO: get from config
            _secretKey = "testsecretKeyabc$"; //TODO: get from config
        }

        public async Task Initialize()
        {
            UserToken = await _localStorageService.GetItemAsync<string>("usertoken");
            CustomRowidCulture = await _localStorageService.GetItemAsync<short>("customrowidculture");
            var selectedConnection = await _localStorageService.GetItemAsync<string>("selectedConnection");
            try
            {
                SelectedConnection = JsonConvert.DeserializeObject<SDKDbConnection>(selectedConnection);
            }
            catch (System.Exception)
            {
            }
            //Console.WriteLine($"UserToken: {UserToken}");
        }

        public async Task Login(string username, string password, short rowIdDBConnection)
        {
            var BLuser = _backendRouterService.GetSDKBusinessModel("BLUser", this);
            if (BLuser == null)
            {
                throw new Exception("Login Service not found");
            }
            var loginRequest = await BLuser.Call("SignIn", username, password, rowIdDBConnection);
            if (loginRequest.Success)
            {
                UserToken = loginRequest.Data;
                await _localStorageService.SetItemAsync("usertoken", UserToken);
            }
            else
            {
                if (loginRequest.Errors != null && loginRequest.Errors.Count > 0)
                {
                    throw new Exception(loginRequest.Errors.FirstOrDefault());
                }
                else
                {
                    throw new Exception("Login failed");
                }
            }
        }

        public async Task Logout()
        {
            UserToken = "";
            _user = null;
            await _localStorageService.RemoveItemAsync("usertoken");
            await _localStorageService.RemoveItemAsync("lastInteraction");
            await _localStorageService.RemoveItemAsync("n_tabs");

            _navigationManager.NavigateTo("login");
        }

        public async Task SetToken(string token)
        {
            UserToken = token;
            await _localStorageService.SetItemAsync("usertoken", UserToken);
        }

        public async Task SetSelectedConnection(SDKDbConnection selectedConnection)
        {
            SelectedConnection = selectedConnection;
            try
            {
                await _localStorageService.SetItemAsync("selectedConnection", JsonConvert.SerializeObject(selectedConnection));
            }
            catch (System.Exception)
            {
            }
        }

        public SDKDbConnection GetSelectedConnection()
        {
            return SelectedConnection;
        }

        public string GetConnectionLogo()
        {
            if(SelectedConnection != null && SelectedConnection.Rowid != 0 && !string.IsNullOrEmpty(SelectedConnection.LogoUrl)){
                return SelectedConnection.LogoUrl;
            }
            return "_content/Siesa.SDK.Frontend/assets/img/login_logo_empresa.png";
        }
        public string GetConnectionStyle()
        {
                return SelectedConnection.StyleUrl;
        }

        public async Task SetCustomRowidCulture(short rowid)
        {
            CustomRowidCulture = rowid;
            await _localStorageService.SetItemAsync("customrowidculture", CustomRowidCulture);
        }

        public async Task RemoveCustomRowidCulture()
        {
            CustomRowidCulture = 0;
            await _localStorageService.RemoveItemAsync("customrowidculture");
        }

        public async Task SetRowidCompanyGroup(short rowid)
        {

            var BLUser = _backendRouterService.GetSDKBusinessModel("BLUser", this);
            if (BLUser == null)
            {
                throw new Exception("Occurio un error");
            }
            var CompanyGroup = await BLUser.Call("ChangeCompanyGroup", rowid);

            if (CompanyGroup.Success)
            {
                await this.SetToken(CompanyGroup.Data);
            }

        }

        public short GetRowidCompanyGroup()
        {
            if (this.User == null)
            {
             return 0;   
            }
            return this.User.RowidCompanyGroup;
        }

        public short GetRoiwdCulture()
        {
            if (CustomRowidCulture > 0)
            {
                return CustomRowidCulture;
            }
            else
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
        }

        public async Task<bool> IsValidToken()
        {
            var token = await _localStorageService.GetItemAsync<string>("usertoken");
            var user = new SDKJWT(_secretKey, _minutesExp).Validate(token);
            return user != null;
        }
    }
}