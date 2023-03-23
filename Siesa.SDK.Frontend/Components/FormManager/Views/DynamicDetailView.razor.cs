using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Frontend.Extension;

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

        protected override void SetParameters(dynamic businessObj, string businessName){
            parameters.Clear();
            parameters.Add("BusinessObj", businessObj);
            parameters.Add("BusinessName", businessName);
            parameters.Add("IsSubpanel", IsSubpanel);
            parameters.Add("ShowTitle", ShowTitle);
            parameters.Add("ShowButtons", ShowButtons);
            parameters.Add("ShowDeleteButton", ShowDeleteButton);
            if (IsSubpanel)
            {
                parameters.Add("SetTopBar", false);
                parameters.Add("ViewdefName", "related_detail");
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            

            bool changeBusinessObjId = parameters.DidParameterChange(nameof(BusinessObjId), BusinessObjId);
            bool changeBusinessName = parameters.DidParameterChange(nameof(BusinessName), BusinessName);

            await base.SetParametersAsync(parameters);
            if(BusinessObjId !=null && (changeBusinessObjId || changeBusinessName)){
                ErrorMsg = "";
                await InitDetail(Convert.ToInt64(BusinessObjId));
                StateHasChanged();
            }
        }
    }
}