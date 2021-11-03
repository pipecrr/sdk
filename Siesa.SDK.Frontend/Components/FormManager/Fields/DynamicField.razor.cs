using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Frontend.Components.FormManager.Fields
{
    public partial class DynamicField : ComponentBase
    {
        [Parameter] public FieldOptions FieldOpt { get; set; }
        [Parameter] public object ModelObj { get; set; }

        private object fieldModelObj { get; set; }

        private string fieldName { get; set; }

        private FieldTypes FieldType { get; set; }
        private string UnknownFieldType { get; set; }

        private void initField()
        {
            var field = FieldOpt.InitField(ModelObj);
            fieldModelObj = field.ModelObj;
            fieldName = field.Name;
            FieldType = field.FieldType;
            UnknownFieldType = field.UnknownFieldType;
            StateHasChanged();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            initField();       
        }
    }
}
