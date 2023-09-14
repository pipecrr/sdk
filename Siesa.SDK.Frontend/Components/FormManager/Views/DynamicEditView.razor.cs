using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Frontend.Extension;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Newtonsoft.Json;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DynamicEditView : DynamicBaseViewModel
    {

        [Parameter] public Dictionary<string, object> DefaultFields { get; set; }

        public dynamic ParentBaseObj { get; set; }

        [Inject]
        public IBackendRouterService BackendRouterService { get; set; }
        private FormViewModel FormViewModel { get; set; } = new FormViewModel();
        private List<string> _extraFields = new List<string>();

        private void GetExtraFields(string bName = null)
        {
            try
            {
                string viewdefName = "detail";

                if (IsSubpanel)
                {
                    viewdefName = "related_detail";
                }

                var metadata = BackendRouterService.GetViewdef(bName, viewdefName);
                if (IsSubpanel && String.IsNullOrEmpty(metadata)){
                    metadata = BackendRouterService.GetViewdef(bName, "related_default");
                }
                if(String.IsNullOrEmpty(metadata))
                {
                    metadata = BackendRouterService.GetViewdef(bName, "default");
                }

                try
                {
                    FormViewModel = JsonConvert.DeserializeObject<FormViewModel>(metadata);
                }
                catch (JsonSerializationException)
                {
                    //Soporte a viewdefs anteriores
                    var panels = JsonConvert.DeserializeObject<List<Panel>>(metadata);
                    FormViewModel.Panels = panels;
                }

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
                
                var baseObj = BusinessObj.BaseObj;
                List<string> extraFieldsTmp = new ();
                foreach (string field in _extraFields){
                    var property = baseObj.GetType().GetProperty(field);
                    if(property != null && property.PropertyType.IsClass && !property.PropertyType.IsPrimitive && !property.PropertyType.IsEnum && property.PropertyType != typeof(string) && property.PropertyType != typeof(byte[])){
                        var rowidNameField = "Rowid"+field;
                        if(!_extraFields.Contains(rowidNameField)){
                            extraFieldsTmp.Add(rowidNameField);
                        }
                    }
                }
                if(extraFieldsTmp.Count > 0){
                    _extraFields = _extraFields.Union(extraFieldsTmp).ToList();
                }
            }
            catch (Exception e)
            {
                ErrorList.Add("Exception: "+e.ToString());
            }
        }
        private async Task InitEdit(Int64 business_obj_id){
            try
            {
                GetExtraFields(BusinessName);
                await BusinessObj.InitializeBusiness(business_obj_id, _extraFields);
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