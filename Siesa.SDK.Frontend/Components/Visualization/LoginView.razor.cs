using System;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Shared.DTOS;

namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class LoginView
{
    [Parameter] public string LogoUrl { get; set; }
    [Parameter] public string ImageUrl { get; set; }
    [Parameter] public int RowidPortal { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    public int RowidCulture = 1;
    public string FormID { get; set; } = Guid.NewGuid().ToString();
    private SDKLoginModelDTO model = new SDKLoginModelDTO();
    private bool loading;
    private bool init_loading = true;
    private bool RecoveringPassword { get; set; } = false;
    private async void HandleValidSubmit(){

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
