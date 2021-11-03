using System;
using System.Threading.Tasks;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class EditView : FormView
    {

        protected override async Task OnInitializedAsync()
        {
            ViewdefName = "edit";
            await base.OnInitializedAsync();
        }

    }
}
