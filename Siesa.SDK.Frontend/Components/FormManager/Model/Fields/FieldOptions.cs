using Siesa.Global.Enums;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.Fields;
using Siesa.SDK.Frontend.Components.FormManager.Fields;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.Utilities;
using System;
using System.Collections.Generic;



namespace Siesa.SDK.Frontend.Components.FormManager.Model.Fields
{
    public class FieldObj
    {
        public object ModelObj { get; set; }
        public string Name { get; set; }
        public FieldTypes FieldType { get; set; }
        public Type SelectFieldType { get; set; }

        public string UnknownFieldType { get; set; }

        public bool IsNullable { get; set; }
        public bool SensitiveData { get; set; }
    }

    public class CustomComponent
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>()
        {
        };

        public bool EvaluateAttributes {get;set;} = true;

        public ICollection<Type> Generics {get; set;} = new List<Type>();
    }

    public class FieldOptions: ICloneable
    {
        public string Name { get; set; }
        public string PropertyName { get; set; }
        public string Placeholder { get; set; } = "";
        public Boolean PureField { get; set; } = false;
        public string Id { get; set; }
        public string Value { get; set; }
        public string Msg { get; set; }

        public string ResourceTag { get; set; }

        public bool Disabled { get; set; } = false;
        public string CssClass { get; set; }
        public FieldTypes FieldType { get; set; }
        public string CustomType { get; set; }

        public bool IsNullable { get; set; } = false;

        public string ViewContext { get; set; }

        public Dictionary<string, object> CustomAttributes { get; set; }
        public int TextFieldCols { get; set; } = 20;
        public int TextFieldRows { get; set; } = 6;

        public bool Hidden { get; set; } = false;
        public bool Required { get; set; } = false;

        public bool SensitiveData { get; set; } = false;

        public int ColumnWidth { get; set; } = 0;

        //Para Listas
        public IEnumerable<object> Options { get; set; }

        public Dictionary<string, int> ColSize { get; set; } = new Dictionary<string, int>()
        {
        };
        public CustomComponent CustomComponent { get; set; }

        //for entity fields
        public string RelatedBusiness { get; set; } = "";
        public Dictionary<string, string> RelatedFilters { get; set; } = new Dictionary<string, string>(){
        };
        public bool AutoValueInUnique { get; set; } = false;

        public List<List<object>> Filters { get; set; }

        public int MinCharsEntityField { get; set; } = 2;
        public string EntityRowidField;

        private FieldObj fieldObj = null;

        public bool ShowLabel { get; set; } = true;

        public FieldObj GetFieldObj(object modelObj, bool force = false)
        {
            if (fieldObj == null || force)
            {
                fieldObj = InitField(modelObj);
            }
            return fieldObj;
        }
        public RelatedParams  RelatedParams { get; set; }

        private List<Type> SupportedTypes = new List<Type>(){
            typeof(SwitchField),
            typeof(SelectBarField<>),
            typeof(TextField),
            typeof(EmailField),
            typeof(RadioButtonField<>)
        };
        private void ChangeCustomType(dynamic attribute)
        {
            if (string.IsNullOrEmpty(CustomType))
            {
                var EnumValue = attribute._customType;
                string customType = Enum.GetName(typeof(CustomTypeField), EnumValue);
                CustomType = customType;
            }
            // else if (CustomType.ToLower().Equals("default"))
            // {
            //     CustomType = string.Empty;
            // }
        }

        private FieldObj InitField(object modelObj)
        {
            FieldObj field = new FieldObj();
            Type originalPropertyType = null; //Used for enums

            if (CustomComponent != null && String.IsNullOrEmpty(CustomType))
            {
                if (String.IsNullOrEmpty(ResourceTag))
                {
                    ResourceTag = $"{modelObj.GetType().Name}.{Name}";
                }
                //Name guid
                if(String.IsNullOrEmpty(Name))
                {
                    Name = Guid.NewGuid().ToString();
                }
                FieldType = FieldTypes.Custom;
                field.Name = Name;
                field.ModelObj = modelObj;
            }
            else
            {             
                //Split Name
                string[] fieldPath = Name.Split('.');
                object currentObject = Utilities.CreateCurrentData(modelObj, fieldPath, typeof(BaseSDK<>));
                field.ModelObj = currentObject;
                field.Name = fieldPath[fieldPath.Length - 1];
                PropertyName = fieldPath[fieldPath.Length - 1];
                if (String.IsNullOrEmpty(ResourceTag))
                {
                    var modelTypeName = field.ModelObj.GetType().Name;
                    if(modelTypeName.EndsWith("DTO"))
                    {
                        modelTypeName = $"DTO.{modelTypeName.Substring(0, modelTypeName.Length - 3)}";
                    }
                    ResourceTag = $"{modelTypeName}.{field.Name}";
                }
                var propertyType = field.ModelObj.GetType().GetProperty(field.Name).PropertyType;
                originalPropertyType = propertyType;
                var customAttr = field.ModelObj.GetType().GetProperty(field.Name).GetCustomAttributes(true);
                if (customAttr != null)
                {
                    foreach (var item in customAttr)
                    {
                        switch (item)
                        {
                            case SDKSensitiveData _:
                                SensitiveData = true;
                                break;
                            case SDKCustomType _:
                                ChangeCustomType(item);
                                break;
                            
                            default:                        
                                break;
                        }
                    }
                }
                //Console.WriteLine(fieldName + " , " + propertyType);
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propertyType = propertyType.GetGenericArguments()[0];
                    IsNullable = true;
                }
                switch (propertyType.Name)
                {
                    case "String":
                        FieldType = FieldTypes.CharField;
                        break;
                    case "Int64":
                        FieldType = FieldTypes.BigIntegerField;
                        break;
                    case "Int32":
                        FieldType = FieldTypes.IntegerField;
                        break;
                    case "Int16":
                        FieldType = FieldTypes.SmallIntegerField;
                        break;
                    case "Byte":
                        FieldType = FieldTypes.ByteField;
                        break;
                    case "Decimal":
                        FieldType = FieldTypes.DecimalField;
                        break;
                    case "DateTime":
                        FieldType = FieldTypes.DateTimeField;
                        break;
                    case "TimeOnly":
                    case "TimeSpan":
                        FieldType = FieldTypes.TimeField;
                        break;
                    case "DateOnly":
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

                var _bannedTypes = new List<Type>() { typeof(string), typeof(byte[]) };

                if (propertyType.IsClass && !propertyType.IsPrimitive && !propertyType.IsEnum && !_bannedTypes.Contains(propertyType))
                {
                    FieldType = FieldTypes.EntityField;
                    if(String.IsNullOrEmpty(EntityRowidField))
                    {
                        var exists = field.ModelObj.GetType().GetProperty($"Rowid{field.Name}");
                        if(exists != null)
                        {
                            if(field.UnknownFieldType.Equals("E00271_AttachmentDetail")){
                                FieldType = FieldTypes.FileField;
                                field.FieldType = FieldTypes.FileField;
                                field.UnknownFieldType = null;
                            }else{
                                EntityRowidField = $"Rowid{field.Name}";
                            }
                        }
                    }
                }

                if (propertyType.IsEnum)
                {
                    FieldType = FieldTypes.SelectField;
                    field.SelectFieldType = originalPropertyType;
                }
            }
            if (!String.IsNullOrEmpty(CustomType) && CustomType.ToLower() != "default")
            {
                Type fieldType = SupportedTypes.Find(x => x.Name.Split("`")[0] == CustomType);
                if (fieldType != null)
                {
                    CustomComponent = new CustomComponent();
                    CustomComponent.Name = fieldType.Name;
                    CustomComponent.Namespace = fieldType.Namespace;
                    CustomComponent.EvaluateAttributes = false;
                    CustomComponent.Attributes.Add("BindModel",field.ModelObj);
                    CustomComponent.Attributes.Add("FieldOpt", this);
                    CustomComponent.Attributes.Add("FieldName",field.Name);

                    if(fieldType.IsGenericType && originalPropertyType != null){
                        CustomComponent.Generics.Add(originalPropertyType);
                    }
                    FieldType = FieldTypes.Custom;
                }
            }
            field.FieldType = FieldType;
            field.IsNullable = IsNullable;
            field.SensitiveData = SensitiveData;
            return field;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }


}
