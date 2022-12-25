using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Frontend.Components.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Fields
{
    public partial class DynamicField : ComponentBase
    {
        [Parameter] public string BLParentBusinessName { get; set; }
        [Parameter] public FieldOptions FieldOpt { get; set; }
        [Parameter] public object ModelObj { get; set; }
        [Parameter] public bool ValidateField { get; set; } = true;
        [CascadingParameter] FormView formView { get; set; }
        public SDKFileField UploadComponent { get; set; }
        private object fieldModelObj { get; set; }

        private string fieldName { get; set; }

        private FieldTypes FieldType { get; set; }
        private string UnknownFieldType { get; set; }

        private Type SelectFieldType { get; set; }
        private Type SelectFieldDetailType { get; set; }

        private IDictionary<string, object> parameters = new Dictionary<string, object>();

        private bool IsNullable { get; set; }

        private void initField(bool force = false)
        {
            var field = FieldOpt.GetFieldObj(ModelObj, force);
            fieldModelObj = field.ModelObj;
            fieldName = field.Name;
            FieldType = field.FieldType;
            UnknownFieldType = field.UnknownFieldType;
            IsNullable = field.IsNullable;
            if(field.SelectFieldType != null)
            {
                SelectFieldType = typeof(SelectField<>).MakeGenericType(field.SelectFieldType);
                SelectFieldDetailType = typeof(SelectFieldDetail<>).MakeGenericType(field.SelectFieldType);
                parameters.Clear();

                parameters.Add("BindModel", fieldModelObj);
                parameters.Add("FieldName", fieldName);
                parameters.Add("FieldOpt", FieldOpt);
                //parameters.Add("TItem", field.SelectFieldType);
            }

            StateHasChanged();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            initField(true);
        }
        public void AddUploadComponent(SDKFileField uploadComponent, string fieldName){
            Dictionary<string, SDKFileField> uploadComponents = formView.FileFields;
            if(uploadComponent != null){
                if(!uploadComponents.ContainsKey(fieldName)){
                    uploadComponents.Add(fieldName, uploadComponent);
                }
            }
        }
    }
}
