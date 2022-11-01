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
        protected override async Task OnInitializedAsync()
        {
            DefaultViewdefName = String.IsNullOrEmpty(DefaultViewdefName) ? "edit" : DefaultViewdefName;
            await base.OnInitializedAsync();
        }

        protected override async Task CheckPermissions()
        {
            await base.CheckPermissions();
            if(!CanEdit)
            {
                NotificationService.ShowError("Custom.Generic.Unauthorized");
                NavigationService.NavigateTo("/", replace:true);
            }
        }

    }
}
