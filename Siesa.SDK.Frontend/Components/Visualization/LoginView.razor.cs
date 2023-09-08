using System;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Shared.DTOS;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class LoginView
{
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IAuthenticationService AuthenticationService { get; set; }
    [Inject] public IBackendRouterService BackendRouterService { get; set; }
    [Inject] public SDKNotificationService SDKNotificationService { get; set; }
    [Inject] private IHttpContextAccessor _contextAccesor { get; set; }
    [Inject] public IJSRuntime JSRuntime { get; set; }
    [Parameter] public string LogoUrl { get; set; }
    [Parameter] public string ImageUrl { get; set; }
    [Parameter] public int RowidConexion { get; set; }

    /// <summary>
    /// User Token for recover password
    /// </summary>
    [Parameter] public string? UserToken { get; set; }
    public short RowidCulture { get; set; } = 1;
    public string FormID { get; set; } = Guid.NewGuid().ToString();
    private string PortalName { get; set; }
    private SDKLoginModelDTO model = new SDKLoginModelDTO();
    private bool loading;
    private bool init_loading = true;
    private bool RecoveringPassword;
    private SDKDbConnection SelectedConnection = new ();
    private string error;
    private bool PortalValid;
    private E00021_Culture selectedCulture { get; set; }
    private List<E00021_Culture> cultures = new List<E00021_Culture>();
    private bool IsOpen { get; set; } = false;
    private string classCulture = "";
    protected override async Task OnInitializedAsync()
    {
        init_loading = true;
        if (AuthenticationService.PortalUser != null)
        {
            NavigationManager.NavigateTo($"/{PortalName}");
            return;
        }
        await InitLogin();
    }

    public async Task<bool> ValidatePortal()
    {

        string HostName = _contextAccesor.HttpContext?.Request.Host.Host;

        var BLSDKPortalUser = BackendRouterService.GetSDKBusinessModel("BLSDKPortalUser", AuthenticationService);
        var result = await BLSDKPortalUser.Call("ValidatePortal", PortalName, SelectedConnection.Rowid, HostName).ConfigureAwait(true);
        if(result.Success && result.Data != null){
            return true;
        }else{
            return false;
        }
    }

    private async Task InitLogin()
    {
        var BLSDKPortalUser = BackendRouterService.GetSDKBusinessModel("BLSDKPortalUser", AuthenticationService);
        var result = await BLSDKPortalUser.Call("GetDBConnection", RowidConexion);
        await getCultures().ConfigureAwait(true);
        getSelectedCulture();
        await GetUserlang().ConfigureAwait(true);
        init_loading = false;
        if(result.Success && result.Data != null){
            SelectedConnection = result.Data;
            await base.OnInitializedAsync();
            StateHasChanged();
        }else{
            StateHasChanged();
        }
    }
    
    /// <summary>
    /// Get user language from browser or database
    /// </summary>
    /// <exception cref="Exception"></exception>
    private async Task GetUserlang()
    {
        short userlang;
        try
        {
            userlang = AuthenticationService.GetRoiwdCulture();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.ToString());
        }

        if (cultures == null)
        {
            cultures = await getCulturesAsync().ConfigureAwait(true);

        }
        try
        {
            if (userlang == 0)
            {
                var browserLang = await JSRuntime.InvokeAsync<string>("getBrowserLang").ConfigureAwait(true);
                string[] language = browserLang.Split('-');
                selectedCulture = cultures.FirstOrDefault(x => string.Equals(x.LanguageCode.ToUpperInvariant(),language[0].ToUpperInvariant(),StringComparison.Ordinal) && string.Equals(x.CountryCode.ToUpperInvariant(), language[1].ToUpperInvariant(),StringComparison.Ordinal));
            }
            else
            {
                selectedCulture = cultures.FirstOrDefault(x => x.Rowid == userlang);
            }
        }catch(Exception ex)
        {
        }

        if (selectedCulture == null){
            cultureDefault();
            try {

                await AuthenticationService.RemoveCustomRowidCulture().ConfigureAwait(true);
            }catch(Exception ex){
                
            }
        }else{
            await OnChangeCulture(selectedCulture.Rowid).ConfigureAwait(true);
        }
    }

    private async void HandleValidSubmit(){
        string message = "BLUser.Login.Message.SuccesLogin";
        loading = true;
        if(SelectedConnection == null)
        {
            SDKNotificationService.ShowError("BLUser.Login.Message.SelectDbConnection", culture: RowidCulture);
            loading = false;
            StateHasChanged();
            return;
        }

        if(string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
        {
            error = "BLLogin.EnterUserName";
            SDKNotificationService.ShowError("BLUser.Login.Message.EnterData", culture: RowidCulture);
            loading = false;
            StateHasChanged();
            return;
        }

        try
        {
            await AuthenticationService.LoginPortal(model.Username, model.Password,SelectedConnection.Rowid);
            if (AuthenticationService.PortalUser != null){
                string urlReturn = $"/Portal/{RowidConexion}";
                NavigationManager.NavigateTo(urlReturn);
            }else{
                SDKNotificationService.ShowError("BLUser.Login.Message.InvalidCredentials", culture: RowidCulture);   
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
            SDKNotificationService.ShowError("BLUser.Login.Message.InvalidCredentials", culture: RowidCulture);
            loading = false;
            StateHasChanged();
        }
    }
    private async void HandleInvalidSubmit(){

    }
    private string GetCssLoding()
    {
        if (!init_loading)
        {
            return "login_loading_close";
        }
        return "";
    }

    private void getSelectedCulture()
    {
        if (cultures != null)
        {
            selectedCulture = cultures.FirstOrDefault(x => x.Rowid == RowidCulture);
        }
        else
        {
            cultureDefault();
        }
        classCulture = $"fi fi-{GetCountryFlagCode(selectedCulture)} fis rounded-circle";
    }
    private async Task getCultures()
    {
        cultures = await getCulturesAsync();
    }
    private async Task<List<E00021_Culture>> getCulturesAsync(){
        List<E00021_Culture> _cultures = new List<E00021_Culture>();
                        var cultureBL = BackendRouterService.GetSDKBusinessModel("BLCulture", AuthenticationService);
        if (cultureBL == null)
        {
            throw new Exception("Culture Service not found");
        }
        var _Allcultures = await cultureBL.GetData(null, null);
        if (_Allcultures != null)
        {
            _cultures = _Allcultures.Data.Select(x => JsonConvert.DeserializeObject<E00021_Culture>(x)).ToList();
        }
        return _cultures;
    }
    private void cultureDefault(){
        selectedCulture = new E00021_Culture()
            {
                Rowid = 1,
                CountryCode = "co",
                Description = "Colombia",
                LanguageCode = "Es-co"
            };
    }

    private void ForgotPassword()
    {
        RecoveringPassword = true;
        StateHasChanged();
    }
    private string GetCountryFlagCode (E00021_Culture culture)
    {
        if(!string.IsNullOrEmpty(culture.CountryCode))
        {
            return culture.CountryCode;
        }

        switch (culture.LanguageCode)
        {
            case "en":
                return "us";
            case "es":
                return "co";
            case "fr":
                return "fr";
            case "de":
                return "de";
            case "it":
                return "it";
            case "pt":
                return "pt";
            case "ru":
                return "ru";
            case "zh":
                return "cn";
            case "ja":
                return "jp";
            case "ko":
                return "kr";
            default:
                return "co";
        }
    }
}
