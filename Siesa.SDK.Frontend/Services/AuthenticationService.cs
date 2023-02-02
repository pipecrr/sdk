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
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace Siesa.SDK.Frontend.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private IBackendRouterService _backendRouterService;
        private IHttpContextAccessor _contextAccesor;
        private IJSRuntime _jsRuntime;
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
            IBackendRouterService BackendRouterService,
            IHttpContextAccessor ContextAccessor,
            IJSRuntime jsRuntime
        )
        {
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _backendRouterService = BackendRouterService;
            _contextAccesor = ContextAccessor;
            _minutesExp = 120; //TODO: get from config
            _secretKey = "testsecretKeyabc$"; //TODO: get from config
            _jsRuntime = jsRuntime;
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
            try
            {
                await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Siesa.SDK.Frontend/js/utils.js");
            }catch (Exception)
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

            string ipAddress = _contextAccesor.HttpContext.Connection.RemoteIpAddress?.ToString();
            string browserName = _contextAccesor.HttpContext.Request.Headers["User-Agent"].ToString();

            var loginRequest = await BLuser.Call("SignInSession", new Dictionary<string, dynamic> {
                {"username", username},
                {"password", password},
                {"rowIdDBConnection", rowIdDBConnection},
                {"ipAddress", ipAddress},
                {"browserName", browserName}
            });
            if (loginRequest.Success)
            {
                UserToken = loginRequest.Data.Token;
                await _localStorageService.SetItemAsync("usertoken", UserToken);
                await SetCookie("sdksession", loginRequest.Data.IdSession);
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
            await RemoveCookie("sdksession");

            _navigationManager.NavigateTo("login");
        }

        public async Task SetToken(string token, bool saveLocalStorage = true)
        {
            UserToken = token;
            if(saveLocalStorage)
            {
                await _localStorageService.SetItemAsync("usertoken", UserToken);
            }
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


        public async Task<bool> ForgotPasswordAsync(string email){

            var request = await GetBLUser();

            var result = await request.Call("SendEmailRecoveryPassword", email, SelectedConnection.Rowid);

            if (result.Success)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> ValidateUserToken(int rowidUser){

            var request = await GetBLUser();

            var result = await request.Call("ValidateUserToken", rowidUser,SelectedConnection.Rowid);

            if (result.Success)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ChangePassword(int rowidUser, string NewPassword, string ConfirmPassword)
        {
            var request = await GetBLUser();
            if (!string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(ConfirmPassword))
            {
                if (NewPassword != ConfirmPassword)
                {
                    throw new Exception("Custom.ForgotPassword.ChangePasswordError");
                }else
                {
                    var resultChangePassword = await request.Call("RecoveryPassword",rowidUser,NewPassword,ConfirmPassword,SelectedConnection.Rowid);
                    if (resultChangePassword.Success)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task<dynamic> GetBLUser()
        {
            var BLUser = _backendRouterService.GetSDKBusinessModel("BLUser", this);

            if (BLUser == null)
            {
                throw new Exception("Custom.Generic.Message.Error");
            }
            return BLUser;
        }

        private async Task SetCookie(string key, string value)
        {
            //execut javascript to set cookie
            try
            {
                await _jsRuntime.InvokeVoidAsync("window.createCookie", key, value);
            }
            catch (System.Exception)
            {
            }
        }

        private async Task RemoveCookie(string key)
        {
            //execut javascript to set cookie
            try
            {
                await _jsRuntime.InvokeVoidAsync("window.deleteCookie", key);
            }
            catch (System.Exception)
            {
            }
        }
    }
}