using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Frontend.Components.FormManager.Shared.Fields
{
    public partial class DynamicField : ComponentBase
    {
        [Parameter] public FieldOptions FieldOpt { get; set; }
        [Parameter] public object ModelObj { get; set; }

        private object fieldModelObj { get; set; }

        private string fieldName { get; set; }

        private string FieldType { get; set; }

        //public string GetProp(string prop)
        //{
        //    return (string)fieldModelObj.GetType().GetProperty(prop).GetValue(fieldModelObj, null);
        //}

        private void initField()
        {
            //Split FieldOpt.Name
            string[] fieldPath = FieldOpt.Name.Split('.');
            //loop through the path
            object currentObject = ModelObj;
            for (int i = 0; i < (fieldPath.Length - 1); i++)
            {
                var tmpType = currentObject.GetType();
                var tmpProperty = tmpType.GetProperty(fieldPath[i]);
                var tmpValue = tmpProperty.GetValue(currentObject, null);
                var isEntity = tmpProperty.PropertyType.IsSubclassOf(typeof(BaseEntity));
                if (tmpValue == null && isEntity)
                {
                    tmpValue = Activator.CreateInstance(tmpProperty.PropertyType);
                    tmpProperty.SetValue(currentObject, tmpValue);
                }
                currentObject = tmpValue;
            }
            fieldModelObj = currentObject;
            fieldName = fieldPath[fieldPath.Length - 1];

            if (FieldOpt.FieldType != null && FieldOpt.FieldType != "")
            {
                FieldType = FieldOpt.FieldType;
            }
            else
            {
                var propertyType = fieldModelObj.GetType().GetProperty(fieldName).PropertyType;
                //Console.WriteLine(fieldName + " , " + propertyType);
                switch (propertyType.Name)
                {
                    case "String":
                        FieldType = "CharField";
                        break;
                    case "Int32":
                        FieldType = "IntegerField";
                        break;
                    case "Decimal":
                        FieldType = "DecimalField";
                        break;
                    case "DateTime":
                        FieldType = "DateTimeField";
                        break;
                    case "Boolean":
                        FieldType = "BooleanField";
                        break;
                    case "EntityTextField":
                        FieldType = "TextField";
                        break;
                    default:
                        FieldType = propertyType.Name;
                        break;
                }
                StateHasChanged();
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            initField();       
        }
    }
}
