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
            ViewdefName = "edit";
            await base.OnInitializedAsync();
        }

    }
}
