using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.Global.Enums;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.Fields;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class CreateView : FormView
    {
        [Parameter]
        public bool SetTopBar { get; set; } = true;
        [Parameter]
        public string BLNameParentAttatchment { get; set; }
        
        private DynamicViewType _viewContext { get; set; } = DynamicViewType.Create;

        protected override async Task OnInitializedAsync()
        {
            DefaultViewdefName = String.IsNullOrEmpty(DefaultViewdefName) ? "create" : DefaultViewdefName;
            if (IsTableA)
            {
                await InitViewTableA().ConfigureAwait(true);
            }
            await BusinessObj.InstanceDynamicEntities(BusinessName);
            
            await base.OnInitializedAsync().ConfigureAwait(true);
        }

        protected override async Task CheckPermissions()
        {
            if(IsSubpanel && BusinessName.Equals("BLAttachmentDetail", StringComparison.Ordinal))
            {
                try
                {
                    CanCreate = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.UploadAttachment, AuthenticationService).ConfigureAwait(true);
                }
                catch (System.Exception ex)
                {
                    ErrorList.Add(new Shared.DTOS.ModelMessagesDTO()
                    {
                        Message = "Custom.Generic.Message.Error",
                        StackTrace = ex.StackTrace
                    });
                }
            }else
            {
                await base.CheckPermissions().ConfigureAwait(true);
            }
            if(!CanCreate)
            {
                _ = NotificationService.ShowError("Custom.Generic.Unauthorized");
                ErrorMsg = "Custom.Generic.Unauthorized";
                ErrorList.Add(new Shared.DTOS.ModelMessagesDTO()
                {
                    Message = "Custom.Generic.Unauthorized"
                });
                
                if(!IsSubpanel){
                    // NavigationService.NavigateTo("/", replace:true);
                }
            }
        }
    }
}
