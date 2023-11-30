using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class EditView : FormView
    {
        [Parameter]
        public bool SetTopBar { get; set; } = true;
        
        private DynamicViewType _viewContext { get; set; } = DynamicViewType.Edit;
        protected override async Task OnInitializedAsync()
        {
            DefaultViewdefName = String.IsNullOrEmpty(DefaultViewdefName) ? "edit" : DefaultViewdefName;
            if (IsTableA)
            {
                await InitViewTableA().ConfigureAwait(true);
            }
            await BusinessObj.InstanceDynamicEntities(BusinessName);
            
            await base.OnInitializedAsync().ConfigureAwait(true);
        }

        protected override async Task CheckPermissions()
        {
            await base.CheckPermissions().ConfigureAwait(true);
            if(!CanEdit)
            {
                _ = NotificationService.ShowError("Custom.Generic.Unauthorized");
                ErrorMsg = "Custom.Generic.Unauthorized";
                if(!IsSubpanel){
                    // NavigationService.NavigateTo("/", replace:true);
                }
            }
        }
    }
}
