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
using Siesa.Global.Enums;
using Siesa.SDK.Frontend.Criptography;

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
        private ISDKJWT _sdkJWT;

        private short CustomRowidCulture = 0;
        public short RowidCultureChanged { get; set; } = 0;

        private SDKDbConnection SelectedConnection = new SDKDbConnection();

        public string UserToken { get; private set; } = "";
        public string PortalUserToken { get; private set; } = "";

        private int _selectedSuite;

        private short _rowIdCompanyGroup = 0;

        private string _userPhoto = "";
        private string _portalUserPhoto = "";
        private string _logoPhoto = "";
        private string _portalLogoPhoto = "";

        
        public UserPreferencesDTO UserPreferences { get; set; } = new UserPreferencesDTO();
        public UserPreferencesDTO PortalUserPreferences { get; set; } = new UserPreferencesDTO();

        private JwtUserData? _user;
        private JwtUserData? _portalUser;
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
                    try
                    {
                        _user = _sdkJWT.Validate<JwtUserData>(UserToken);
                    }
                    catch (System.Exception)
                    {
                        _user = null;
                    }
                }
                return _user;
            }
        }

        public JwtUserData PortalUser
        {
            get
            {
                if (PortalUserToken == "")
                {
                    return null;
                }
                if (_portalUser == null)
                {
                    _portalUser = new SDKJWT(_secretKey, _minutesExp).Validate(PortalUserToken);
                }
                return _portalUser;
            }
        }

        public AuthenticationService(
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            IBackendRouterService BackendRouterService,
            IHttpContextAccessor ContextAccessor,
            IJSRuntime jsRuntime,
            ISDKJWT sdkJWT
        )
        {
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _backendRouterService = BackendRouterService;
            _contextAccesor = ContextAccessor;
            _minutesExp = 120; //TODO: get from config
            _secretKey = "testsecretKeyabc$"; //TODO: get from config
            _jsRuntime = jsRuntime;
            _sdkJWT = sdkJWT;
        }

        public async Task Initialize()
        {
            UserToken = await _localStorageService.GetItemAsync<string>("usertoken");
            PortalUserToken = await _localStorageService.GetItemAsync<string>("portalusertoken");
            CustomRowidCulture = await _localStorageService.GetItemAsync<short>("customrowidculture");
            _selectedSuite = await _localStorageService.GetItemAsync<int>("selectedSuite");
            //_rowIdCompanyGroup = await _localStorageService.GetItemAsync<short>("rowIdCompanyGroup");
            _userPhoto = await _localStorageService.GetItemAsync<string>("userPhoto");
            _logoPhoto = await _localStorageService.GetItemAsync<string>("imageCompanyGroup");
            var selectedConnection = await _localStorageService.GetItemAsync<string>("selectedConnection");
            UserPreferences = await _localStorageService.GetItemAsync<UserPreferencesDTO>("userPreferences");

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
                await SetUserPhoto(loginRequest.Data.UserPhoto);
                await SetPreferencesUser(loginRequest.Data.UserPreferences);
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

        public async Task LoginPortal(string username, string password, short rowIdDBConnection, bool IsUpdateSession = false, short rowIdCompanyGroup = 1)
        {
            var BLSDKPortalUser = _backendRouterService.GetSDKBusinessModel("BLSDKPortalUser", this);
            if (BLSDKPortalUser == null)
            {
                throw new Exception("Login Service not found");
            }

            short LastCompanyGroupSelected = await _localStorageService.GetItemAsync<short>("rowidCompanyGroup");

            if(LastCompanyGroupSelected > 0 && LastCompanyGroupSelected != rowIdCompanyGroup) 
            {
                rowIdCompanyGroup = LastCompanyGroupSelected;
            }
            
            var loginRequest = await BLSDKPortalUser.Call("SignInSessionPortal", new Dictionary<string, dynamic> {
                {"username", username},
                {"password", password},
                {"rowIdDBConnection", rowIdDBConnection},
                {"rowidCulture", RowidCultureChanged},
                {"rowIdCompanyGroup", rowIdCompanyGroup}
            });
            if (loginRequest.Success)
            {
                PortalUserToken = loginRequest.Data.Token;
                await _localStorageService.SetItemAsync("portalusertoken", PortalUserToken);
                await SetCookie("selectedConnection", rowIdDBConnection.ToString());
                await SetPortalUserPhoto(loginRequest.Data.UserPhoto);
                await SetPreferencesPortalUser(loginRequest.Data.UserPreferences);
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
            await _localStorageService.RemoveItemAsync("userPhoto");
            await _localStorageService.RemoveItemAsync("userPreferences");
            //await _localStorageService.RemoveItemAsync("selectedSuite");
            await RemoveCookie("sdksession");
            await RemoveCookie("selectedConnection");
            UserToken = "";
            _user = null;

            _navigationManager.NavigateTo("login");

        }

        public async Task LogoutPortal()
        {
            SDKDbConnection _selectedConnection = GetSelectedConnection();
            var RowIdDBConnection = _selectedConnection != null ? _selectedConnection.Rowid : 0;
            
            await _localStorageService.RemoveItemAsync("portalusertoken");
            await _localStorageService.RemoveItemAsync("portaluserPhoto");
            await _localStorageService.RemoveItemAsync("portalUserPreferences");
            await RemoveCookie("selectedConnection");
            PortalUserToken = "";
            _portalUser = null;

            _navigationManager.NavigateTo($"Portal/{RowIdDBConnection}");
        }

        public async Task SetToken(string token, bool saveLocalStorage = true)
        {
            UserToken = token;
            if(saveLocalStorage)
            {
                await _localStorageService.SetItemAsync("usertoken", UserToken);
            }
        }

        public async Task SetTokenPortal(string token, bool saveLocalStorage = true)
        {
            PortalUserToken = token;
            if(saveLocalStorage)
            {
                await _localStorageService.SetItemAsync("portalusertoken", PortalUserToken);
            }
        }

        public async Task SetUserPhoto(string _data, bool saveLocalStorage = true)
        {
            _userPhoto = _data;

            if(saveLocalStorage)
            {
                await _localStorageService.SetItemAsync("userPhoto", _userPhoto);
            }
        }

        public async Task SetPortalUserPhoto(string _data, bool saveLocalStorage = true)
        {
            _portalUserPhoto = _data;

            if(saveLocalStorage)
            {
                await _localStorageService.SetItemAsync("portaluserPhoto", _portalUserPhoto);
            }
        }

        public async Task SetConnectionLogo(string _data, bool saveLocalStorage = true)
        {
            _logoPhoto = _data;

            if(saveLocalStorage)
            {
                await _localStorageService.SetItemAsync("imageCompanyGroup", _logoPhoto);
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

        public async Task<string> FetchConnectionLogo(short rowidCompanyGroup = 0)
        {
            var respose = "";
            if (rowidCompanyGroup > 0)
            {
                E00200_CompanyGroup SelectedGroup = new E00200_CompanyGroup();

                var BLCompanyGroup = _backendRouterService.GetSDKBusinessModel("BLSDKCompanyGroup",this);

                var companyGroup = await BLCompanyGroup.Call("GetCompanyGroupLogo",rowidCompanyGroup);

                if (companyGroup.Success)
                {
                    SelectedGroup = companyGroup.Data;
                    
                    if (SelectedGroup.Logo?.FileInternalAttached != null && SelectedGroup.Logo?.FileType != null)
                    {
                        string ImageBase64 = Convert.ToBase64String(SelectedGroup.Logo.FileInternalAttached);
                    
                        respose = $"data:{SelectedGroup.Logo.FileType};base64,{ImageBase64}";

                    }else
                    {
                        respose = "_content/Siesa.SDK.Frontend/assets/img/LogoSiesaNoSub.svg";
                    }
                }
            }

            return respose;
        }

        public string GetUserPhoto()
        {
            if (string.IsNullOrEmpty(_userPhoto))
            {
                _userPhoto = "_content/Siesa.SDK.Frontend/assets/img/Profile_default.png";
            }

            return _userPhoto;
        }

        public string GetConnectionLogo()
        {
            if (string.IsNullOrEmpty(_logoPhoto))
            {
                _logoPhoto = "_content/Siesa.SDK.Frontend/assets/img/LogoSiesaNoSub.svg";
            }

            return _logoPhoto;
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
                var _connectionLogo = await FetchConnectionLogo(rowid);
                await this.SetConnectionLogo(_connectionLogo);
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
            try
            {
                var token = await _localStorageService.GetItemAsync<string>("usertoken");
                var user =   _sdkJWT.Validate<JwtUserData>(token);
                //var user = new SDKJWT(_secretKey, _minutesExp).Validate(token);
                return user != null;
            }
            catch (System.Exception)
            {

                return false;
            }
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
        public async Task SetPreferencesUser(UserPreferencesDTO _userPreferencesDTO)
        {
            await _localStorageService.SetItemAsync("userPreferences", _userPreferencesDTO);
            UserPreferences = _userPreferencesDTO;
        }
        public async Task SetPreferencesPortalUser(UserPreferencesDTO _userPreferencesDTO)
        {
            await _localStorageService.SetItemAsync("portalUserPreferences", _userPreferencesDTO);
            PortalUserPreferences = _userPreferencesDTO;
        }

        public UserPreferencesDTO GetPreferencesUser()
        {
            if (UserPreferences == null)
            {
                UserPreferences = new UserPreferencesDTO();
            }
            
            return UserPreferences;
        }
    }
}