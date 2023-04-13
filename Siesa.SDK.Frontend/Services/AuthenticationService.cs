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
        public short RowidCultureChanged { get; set; } = 0;

        private SDKDbConnection SelectedConnection = new SDKDbConnection();

        public string UserToken { get; private set; } = "";

        private int _selectedSuite;

        private short _rowIdCompanyGroup = 0;

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
            _selectedSuite = await _localStorageService.GetItemAsync<int>("selectedSuite");
            //_rowIdCompanyGroup = await _localStorageService.GetItemAsync<short>("rowIdCompanyGroup");
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

        public async Task Login(string username, string password, short rowIdDBConnection, 
        bool IsUpdateSession = false, short rowIdCompanyGroup = 1)
        {
            var BLuser = _backendRouterService.GetSDKBusinessModel("BLUser", this);
            if (BLuser == null)
            {
                throw new Exception("Login Service not found");
            }

            //Sacar la IP verdadera del Header**

            string ipAddress = _contextAccesor.HttpContext.Connection.RemoteIpAddress?.ToString();

            string browserName = _contextAccesor.HttpContext.Request.Headers["User-Agent"].ToString();

            string sessionId = IsUpdateSession ? _contextAccesor.HttpContext.Request.Cookies["sdksession"].ToString() : "";

            short LastCompanyGroupSelected = await _localStorageService.GetItemAsync<short>("rowidCompanyGroup");

            if(LastCompanyGroupSelected > 0 && LastCompanyGroupSelected != rowIdCompanyGroup) 
            {
                rowIdCompanyGroup = LastCompanyGroupSelected;
            }

            
            var loginRequest = await BLuser.Call("SignInSession", new Dictionary<string, dynamic> {
                {"username", username},
                {"password", password},
                {"rowIdDBConnection", rowIdDBConnection},
                {"ipAddress", ipAddress},
                {"browserName", browserName},
                {"rowidCulture", RowidCultureChanged},
                {"sessionId", sessionId},
                {"IsUpdateSession", IsUpdateSession},
                {"rowIdCompanyGroup", rowIdCompanyGroup}
            });
            if (loginRequest.Success)
            {
                UserToken = loginRequest.Data.Token;
                var sdksesion = loginRequest.Data.IdSession;
                await _localStorageService.SetItemAsync("usertoken", UserToken);
                await SetCookie("sdksession", loginRequest.Data.IdSession);
                await SetCookie("selectedConnection", rowIdDBConnection.ToString());
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

        //RenewToken method
        public async Task RenewToken()
        {
            var sessionId = "";
            
            _contextAccesor.HttpContext.Request.Cookies.TryGetValue("sdksession", out sessionId);

            var BLuser = _backendRouterService.GetSDKBusinessModel("BLUser", this);
            if (BLuser != null)
            {
                var _renewToken = await BLuser.Call("RenewToken",sessionId);
                if (_renewToken.Success)
                {
                    UserToken = _renewToken.Data;
                    await _localStorageService.SetItemAsync("usertoken", UserToken);
                }
            }
        }

        public async Task Logout()
        {
            
            try
            {
                var sessionId = await GetCookie("sdksession");

                var BLSession = _backendRouterService.GetSDKBusinessModel("BLSession", this);

                var updateSession = BLSession.Call("UpdateEndDate",sessionId);

            }catch (Exception e)
            {
            }

            await _localStorageService.RemoveItemAsync("usertoken");
            await _localStorageService.RemoveItemAsync("lastInteraction");
            await _localStorageService.RemoveItemAsync("n_tabs");
            await _localStorageService.RemoveItemAsync("bd");
            //await _localStorageService.RemoveItemAsync("selectedSuite");
            await RemoveCookie("sdksession");
            await RemoveCookie("selectedConnection");
            UserToken = "";
            _user = null;

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

        public async Task<string> GetConnectionLogo(short rowidCompanyGroup = 0)
        {
            string LogoUrl = await _localStorageService.GetItemAsync<string>("imageCompanyGroup");

            E00200_CompanyGroup SelectedGroup = new E00200_CompanyGroup();

            if (string.IsNullOrEmpty(LogoUrl) || rowidCompanyGroup > 0)
            {
                var BLCompanyGroup = _backendRouterService.GetSDKBusinessModel("BLSDKCompanyGroup",this);

                var companyGroup = await BLCompanyGroup.Call("GetCompanyGroupLogo",rowidCompanyGroup);

                if (companyGroup.Success)
                {
                    SelectedGroup = companyGroup.Data;
                    
                    if (SelectedGroup.Logo?.FileInternalAttached != null && SelectedGroup.Logo?.FileType != null)
                    {
                        string ImageBase64 = Convert.ToBase64String(SelectedGroup.Logo.FileInternalAttached);
                    
                        LogoUrl = $"data:{SelectedGroup.Logo.FileType};base64,{ImageBase64}";

                    }else
                    {
                        LogoUrl = "_content/Siesa.SDK.Frontend/assets/img/LogoSiesaNoSub.svg";
                    }
                    
                    await _localStorageService.SetItemAsync("imageCompanyGroup", LogoUrl);
                }
            }



            return LogoUrl;
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
            var sessionId = await GetCookie("sdksession");
            var CompanyGroup = await BLUser.Call("ChangeCompanyGroup", rowid, sessionId);

            if (CompanyGroup.Success)
            {
                await this.SetToken(CompanyGroup.Data);

                await this.GetConnectionLogo(rowid);

                await _localStorageService.SetItemAsync("rowidCompanyGroup", rowid);
            }
        }

        public short GetRowidCompanyGroup()
        {
            short rowid = 0;

            if (this.User != null)
            {
             rowid = this.User.RowidCompanyGroup;
            }

            return rowid;   
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

        //read cookie
        private async Task<string> GetCookie(string key)
        {
            //execut javascript to set cookie
            try
            {
                return await _jsRuntime.InvokeAsync<string>("window.readCookie", key);
            }
            catch (System.Exception)
            {
                return "";
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

        public int GetSelectedSuite()
        {
            return _selectedSuite;
        }

        public void SetSelectedSuite(int rowid)
        {
            _ = _localStorageService.SetItemAsync("selectedSuite", rowid);
            _selectedSuite = rowid;
            
        }

        public Task Login(string username, string password, short rowIdDBConnection, bool IsUpdateSession = false)
        {
            throw new NotImplementedException();
        }
    }
}