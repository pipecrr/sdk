using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Frontend.Extension;
using Siesa.SDK.Shared.DTOS;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DynamicEditView : DynamicBaseViewModel
    {

        [Parameter] public Dictionary<string, object> DefaultFields { get; set; }

        public dynamic ParentBaseObj { get; set; }
        private async Task InitEdit(Int64 business_obj_id){
            try
            {
               
                await BusinessObj.InitializeBusiness(business_obj_id);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error EditViewModel", e.ToString());
                ErrorList.Add(new ModelMessagesDTO()
                {
                    Message = "Custom.Generic.Message.Error",
                    StackTrace = e.StackTrace,
                });
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
            parameters.Add("ShowCancelButton", ShowCancelButton);
            parameters.Add("ShowSaveButton", ShowSaveButton);
            parameters.Add("OnSave", OnSave);
            parameters.Add("OnCancel", OnCancel);
            if (IsSubpanel)
            {
                parameters.Add("SetTopBar", false);
                parameters.Add("ViewdefName", "related_edit");
                if(DefaultFields != null)
                {
                    ParentBaseObj = DefaultFields.Keys.ToList();
                    parameters.Add("ParentBaseObj", ParentBaseObj);
                } 
            }
        }


        public override async Task SetParametersAsync(ParameterView parameters)
        {
            bool changeBusinessObjId = parameters.DidParameterChange(nameof(BusinessObjId), BusinessObjId);
            bool changeBusinessName = parameters.DidParameterChange(nameof(BusinessName), BusinessName);

            await base.SetParametersAsync(parameters);

            if(BusinessObjId != null && (changeBusinessObjId || changeBusinessName)){
                await InitEdit(Convert.ToInt64(BusinessObjId));
                StateHasChanged();
            }
        }
    }
}