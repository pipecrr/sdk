using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Siesa.SDK.Frontend.Components.FormManager.ViewModels
{
    public class DynamicDetailViewModel: DynamicBaseViewModel
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