using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Frontend.Extension;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DynamicDetailView : DynamicBaseViewModel
    {
        [Parameter] 
        public bool AllowDelete { get; set; } = true;
        
        [Inject]
        public IBackendRouterService BackendRouterService { get; set; }
        private FormViewModel FormViewModel { get; set; } = new FormViewModel();

        private List<string> _extraFields = new List<string>();
        private async Task GetExtraFields(string bName = null)
        {
            try
            {
                string _viewdefName = "detail";

                if (IsSubpanel)
                {
                    _viewdefName = "related_detail";
                }

                var metadata = BackendRouterService.GetViewdef(bName, _viewdefName);

                if(String.IsNullOrEmpty(metadata))
                {
                    metadata = BackendRouterService.GetViewdef(bName, "default");
                }

                FormViewModel = JsonConvert.DeserializeObject<FormViewModel>(metadata);

                var defaultFields = FormViewModel.Panels.SelectMany(panel => panel.Fields)
                                    .Where(f=> f.CustomComponent == null && f.Name.StartsWith("BaseObj."))
                                    .Select(field => field.Name)
                                    .ToList(); 

                if (FormViewModel.ExtraFields.Count > 0){
                    _extraFields =  FormViewModel.ExtraFields.Select(f => f)
                    .Union(defaultFields)
                    .ToList();

                    _extraFields = _extraFields.Select(field => field.Replace("BaseObj.", "")).ToList();
                }else{
                    _extraFields = defaultFields.Select(field => field.Replace("BaseObj.", "")).ToList();
                }
                
                var BaseObj = BusinessObj.BaseObj;                
                foreach (string field in _extraFields){
                    var property = BaseObj.GetType().GetProperty(field);
                    if(property != null && BaseObj.GetType().GetProperty(field).PropertyType.IsClass && BaseObj.GetType().GetProperty(field).PropertyType != typeof(string)){
                        var RowidNameField = "Rowid"+field;
                        if(!_extraFields.Contains(RowidNameField)){
                            _extraFields.Add(RowidNameField);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                ErrorList.Add("Exception: "+e.ToString());
            }
        }

        private async Task InitDetail(Int64 business_obj_id)
        {
            await GetExtraFields(BusinessName);
            try
            {
                await BusinessObj.InitializeBusiness(business_obj_id,_extraFields);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error DetailViewModel", e.ToString());
                ErrorMsg = e.ToString();
                ErrorList.Add("Exception: "+e.ToString());
            }
        }

        protected override void SetParameters(dynamic businessObj, string businessName)
        {

            
            parameters.Clear();
            parameters.Add("BusinessObj", businessObj);
            parameters.Add("BusinessName", businessName);
            parameters.Add("IsSubpanel", IsSubpanel);
            parameters.Add("ShowTitle", ShowTitle);
            parameters.Add("ShowButtons", ShowButtons);

            if(!AllowDelete && IsSubpanel)
            {
                ShowDeleteButton = AllowDelete;
            }
            parameters.Add("ShowDeleteButton", ShowDeleteButton);

            if (IsSubpanel)
            {
                parameters.Add("SetTopBar", false);
                parameters.Add("ViewdefName", "related_detail");
                parameters.Add("BLNameParentAttatchment", BLNameParentAttatchment);
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            

            bool changeBusinessObjId = parameters.DidParameterChange(nameof(BusinessObjId), BusinessObjId);
            bool changeBusinessName = parameters.DidParameterChange(nameof(BusinessName), BusinessName);

            await base.SetParametersAsync(parameters);
            if(BusinessObjId !=null && (changeBusinessObjId || changeBusinessName)){
                ErrorMsg = "";
                ErrorList = new List<string>();
                await InitDetail(Convert.ToInt64(BusinessObjId));
                StateHasChanged();
            }
        }
    }
}