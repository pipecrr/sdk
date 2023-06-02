using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.Global.Enums;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class CreateView : FormView
    {
        [Parameter]
        public bool SetTopBar { get; set; } = true;

        [Parameter]
        public string BLNameParentAttatchment { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DefaultViewdefName = String.IsNullOrEmpty(DefaultViewdefName) ? "create" : DefaultViewdefName;
            await BusinessObj.InstanceDynamicEntities(BusinessName);
            await base.OnInitializedAsync();
        }

        protected override async Task CheckPermissions()
        {
            if(IsSubpanel && BusinessName.Equals("BLAttachmentDetail"))
            {
                try
                {
                    CanCreate = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.UploadAttachment, AuthenticationService);
                }
                catch (System.Exception)
                {
                }
            }else
            {
                await base.CheckPermissions();
            }
            if(!CanCreate)
            {
                NotificationService.ShowError("Custom.Generic.Unauthorized");
                ErrorMsg = "Custom.Generic.Unauthorized";
                ErrorList.Add("Custom.Generic.Unauthorized");
                if(!IsSubpanel){
                    // NavigationService.NavigateTo("/", replace:true);
                }
            }
        }
        
    }
}
