using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DynamicDetailView : DynamicBaseViewModel
    {
        private async Task InitDetail(Int64 business_obj_id)
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

                        await InitDetail(Convert.ToInt64(BusinessObjId));
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