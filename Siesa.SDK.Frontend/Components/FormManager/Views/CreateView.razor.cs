using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class CreateView : FormView
    {
        [Parameter]
        public bool SetTopBar { get; set; } = true;        

        protected override async Task OnInitializedAsync()
        {
            DefaultViewdefName = String.IsNullOrEmpty(DefaultViewdefName) ? "create" : DefaultViewdefName;
            await base.OnInitializedAsync();
        }
        
    }
}
