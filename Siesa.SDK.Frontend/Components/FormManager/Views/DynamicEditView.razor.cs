using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DynamicEditView : DynamicBaseViewModel
    {
        private async Task InitEdit(Int64 business_obj_id){
            try
            {
                await BusinessObj.InitializeBusiness(business_obj_id);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error EditViewModel", e.ToString());
                ErrorMsg = e.ToString();
            }
        }

        protected override void SetParameters(dynamic businessObj, string businessName){
            parameters.Clear();
            parameters.Add("BusinessObj", businessObj);
            parameters.Add("BusinessName", businessName);
            parameters.Add("IsSubpanel", IsSubpanel);
            if (IsSubpanel)
            {
                parameters.Add("SetTopBar", false);
                parameters.Add("ViewdefName", "related_edit");
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

                        await InitEdit(Convert.ToInt64(value));
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