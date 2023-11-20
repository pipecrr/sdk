using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Frontend.Extension;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{   
    /// <summary>
    /// Represents a dynamic detail view for displaying and editing business object details.
    /// </summary>
    public partial class DynamicDetailView : DynamicBaseViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether delete functionality is allowed.
        /// </summary>
        [Parameter] 
        public bool AllowDelete { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the injected backend router service for obtaining view definitions.
        /// </summary>
        [Inject]
        public IBackendRouterService BackendRouterService { get; set; }
        private FormViewModel FormViewModel { get; set; } = new FormViewModel();

        private List<string> _extraFields = new List<string>();
        public string DefaultViewdefName { get; set; } = "detail";
        private void GetExtraFields(string bName = null)
        {
            try
            {
                string viewdefName = DefaultViewdefName;

                if (IsSubpanel)
                {
                    viewdefName = "related_detail";
                }

                var metadata = GetMetadata(bName, viewdefName);

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

                var defaultFields = FormViewModel.Panels.SelectMany(panel =>
                    {
                        return GetAllNestedFields(panel.Fields);
                    }).Where(f=> f.CustomComponent == null && f.Name.StartsWith("BaseObj.", StringComparison.Ordinal))
                                    .Select(field => field.Name)
                                    .ToList();

                if (FormViewModel.ExtraFields.Count > 0){
                    _extraFields =  FormViewModel.ExtraFields.Select(f => f)
                    .Union(defaultFields)
                    .ToList();

                    _extraFields = _extraFields.Select(field => field.Replace("BaseObj.", "",StringComparison.Ordinal)).ToList();
                }else{
                    _extraFields = defaultFields.Select(field => field.Replace("BaseObj.", "",StringComparison.Ordinal)).ToList();
                }
                
                var baseObj = BusinessObj.BaseObj;
                List<string> extraFieldsTmp = new ();
                foreach (string field in _extraFields){
                    var property = baseObj.GetType().GetProperty(field);
                    if(property != null && property.PropertyType.IsClass && !property.PropertyType.IsPrimitive && !property.PropertyType.IsEnum && property.PropertyType != typeof(string) && property.PropertyType != typeof(byte[])){
                        var rowidNameField = "Rowid"+field;
                        //check if baseObj has the field "Rowid"+field
                        var rowidNameProperty = baseObj.GetType().GetProperty(rowidNameField);
                        if(rowidNameProperty != null && !_extraFields.Contains(rowidNameField)){
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

        private string GetMetadata(string bName, string viewdefName)
        {
            var metadata = BackendRouterService.GetViewdef(bName, viewdefName);
            if (IsSubpanel && String.IsNullOrEmpty(metadata))
            {
                metadata = BackendRouterService.GetViewdef(bName, "related_default");
            }

            if (String.IsNullOrEmpty(metadata) && viewdefName != DefaultViewdefName)
            {
                metadata = BackendRouterService.GetViewdef(bName, DefaultViewdefName);
            }

            if (String.IsNullOrEmpty(metadata))
            {
                metadata = BackendRouterService.GetViewdef(bName, "default");
            }

            return metadata;
        }

        private List<FieldOptions> GetAllNestedFields(List<FieldOptions> panelFields)
        {
            List<FieldOptions> nestedFields = new List<FieldOptions>();
            foreach (var field in panelFields)
            {
                if (field.Fields != null && field.Fields.Count > 0)
                {
                    nestedFields.AddRange(GetAllNestedFields(field.Fields));
                }
                else
                {
                    nestedFields.Add(field);
                }
            }
            return nestedFields;
        }

        private async Task InitDetail(Int64 businessObjId)
        {
            GetExtraFields(BusinessName);
            try
            {
                await BusinessObj.InitializeBusiness(businessObjId,_extraFields);
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
                await InitDetail(Convert.ToInt64(BusinessObjId));
                StateHasChanged();
            }
        }
    }
}