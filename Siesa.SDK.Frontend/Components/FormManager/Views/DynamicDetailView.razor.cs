using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DynamicDetailView : DynamicBaseViewModel
    {
        private async Task InitDetail(int business_obj_id)
        {
            try
            {
                await BusinessObj.InitializeBusiness(business_obj_id);
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

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var originalBusinessObjId = BusinessObjId;
            var originalBusinessName = BusinessName;
            await base.SetParametersAsync(parameters);
            try
            {

                if (parameters.TryGetValue<string>(nameof(BusinessObjId), out var value))
                {
                    if (value != null && (value != originalBusinessObjId || originalBusinessName != BusinessName))
                    {
                        //BusinessObj = null;
                        ErrorMsg = "";

                        await InitDetail(Convert.ToInt32(BusinessObjId));
                        StateHasChanged();
                    }

                }
            }
            catch (Exception e)
            {

            }
        }
    }
}