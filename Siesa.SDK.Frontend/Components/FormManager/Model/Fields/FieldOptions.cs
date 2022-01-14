using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.FormManager.Fields;
using System;
using System.Collections.Generic;


namespace Siesa.SDK.Frontend.Components.FormManager.Model.Fields
{
    public class FieldObj {
        public object ModelObj { get; set; }
        public string Name { get; set; }
        public FieldTypes FieldType { get; set; }

        public string UnknownFieldType { get; set; }
    }

    public class CustomComponent
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>()
        {
        };

    }

    public class FieldOptions
    {
        public string Name { get; set; }
        public string PropertyName { get; set; }
        public string Placeholder { get; set; } = "";
        public string Label { get; set; }
        public Boolean PureField { get; set; } = false;
        public string Id { get; set; }
        public string Value { get; set; }
        public string Msg { get; set; }

        public bool Disabled { get; set; } = false;
        public string CssClass { get; set; }
        public FieldTypes FieldType { get; set; }

        public string ViewContext { get; set; }

        public List<string> EscucharA { get; set; }

        public Dictionary<string, object> CustomAttributes { get; set; }
        public int TextFieldCols { get; set; } = 20;
        public int TextFieldRows { get; set; } = 6;

        public bool Hidden { get; set; } = false;
        public bool Required { get; set; } = false;

        //Para Listas
        public IEnumerable<ListOption> Options { get; set; }

        public Dictionary<string, int> ColSize { get; set; } = new Dictionary<string, int>()
        {
        };
        public CustomComponent CustomComponent { get; set; }

        public string RelatedBusiness { get; set; } = "";

        public FieldObj InitField(object modelObj)
        {
            FieldObj field = new FieldObj();
            //Split Name
            string[] fieldPath = Name.Split('.');
            //loop through the path
            object currentObject = modelObj;
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
            field.ModelObj = currentObject;
            field.Name = fieldPath[fieldPath.Length - 1];
            PropertyName = fieldPath[fieldPath.Length - 1];

            if (CustomComponent != null)
            {
                FieldType = FieldTypes.Custom;   
            }
            else
            {
                var propertyType = field.ModelObj.GetType().GetProperty(field.Name).PropertyType;
                //Console.WriteLine(fieldName + " , " + propertyType);
                switch (propertyType.Name)
                {
                    case "String":
                        FieldType = FieldTypes.CharField;
                        break;
                    case "Int32":
                        FieldType = FieldTypes.IntegerField;
                        break;
                    case "Decimal":
                        FieldType = FieldTypes.DecimalField;
                        break;
                    case "DateTime":
                        FieldType = FieldTypes.DateField;
                        break;
                    case "Boolean":
                        FieldType = FieldTypes.BooleanField;
                        break;
                    case "EntityTextField":
                        FieldType = FieldTypes.TextField;
                        break;
                    default:
                        FieldType = FieldTypes.Unknown;
                        field.UnknownFieldType = propertyType.Name;
                        break;
                }

                if(propertyType.IsClass && !propertyType.IsPrimitive && !propertyType.IsEnum && propertyType != typeof(string)){
                    FieldType = FieldTypes.EntityField;
                }
            }
            field.FieldType = FieldType;
            return field;
        }
    }

   
}
