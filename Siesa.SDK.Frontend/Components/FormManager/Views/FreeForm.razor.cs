using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class FreeForm : FormView
    {
        [Parameter] public string Viewdef { get; set; }
        [Parameter] public string Title { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ViewdefName = Viewdef;
            await base.OnInitializedAsync();
        }
        
    }
}
