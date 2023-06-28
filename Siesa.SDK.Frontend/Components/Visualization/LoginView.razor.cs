using System;
using Siesa.SDK.Shared.DTOS;


namespace Siesa.SDK.Frontend.Components.Visualization;

public partial class LoginView
{
    public int RowidCulture = 1;
    public string FormID { get; set; } = Guid.NewGuid().ToString();
    private SDKLoginModelDTO model = new SDKLoginModelDTO();
    private bool loading;
    private bool init_loading = true;
    private string LogoUrl { get; set; } = "";
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
