using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DynamicDetailView : DynamicBaseViewModel
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            try
            {
                await BusinessObj.InitializeBusiness(Convert.ToInt32(BusinessObjId));
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error DetailViewModel", e.ToString());
                ErrorMsg = e.ToString();
            }

        }

        public new RenderFragment CreateDynamicComponent() => builder =>
        {
            var viewType = typeof(Views.DetailView);
            builder.OpenComponent(0, viewType);
            builder.AddAttribute(1, "BusinessObj", BusinessObj);
            builder.AddAttribute(2, "BusinessName", BusinessName);
            builder.CloseComponent();
        };
    }
}