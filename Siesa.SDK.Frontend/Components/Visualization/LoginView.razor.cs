using System;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Shared.DTOS;
using System.Threading.Tasks;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class LoginView
{
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IAuthenticationService AuthenticationService { get; set; }
    [Inject] public IBackendRouterService BackendRouterService { get; set; }
    [Inject] public SDKNotificationService SDKNotificationService { get; set; }
    [Parameter] public string LogoUrl { get; set; }
    [Parameter] public string ImageUrl { get; set; }
    [Parameter] public int RowidConexion { get; set; }
    [Parameter] public string PortalName { get; set; }
    public int RowidCulture { get; set; } = 1;
    public string FormID { get; set; } = Guid.NewGuid().ToString();
    private SDKLoginModelDTO model = new SDKLoginModelDTO();
    private bool loading;
    private bool init_loading = true;
    private bool RecoveringPassword;
    private SDKDbConnection SelectedConnection = new ();
    private string error;
    protected override async Task OnInitializedAsync()
    {
        var BLPortalUser = BackendRouterService.GetSDKBusinessModel("BLPortalUser", AuthenticationService);
        var result = await BLPortalUser.Call("GetDBConnection", PortalName, RowidConexion);
        if(result.Success && result.Data != null){
            SelectedConnection = result.Data;
        }
        await base.OnInitializedAsync();
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
            string urlReturn = $"/{PortalName}";
            NavigationManager.NavigateTo(urlReturn);
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
}
