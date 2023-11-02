
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Frontend.Components.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Fields
{
    public partial class DynamicComponentField<TItem> : ComponentBase
    {   
        [Parameter] public TItem Context { get; set; }
        [Parameter] public string Property { get; set; }
        [Parameter] public bool IsEditable { get; set; }
        [Parameter] public Action<TItem, object> OnChangeColumn { get; set; }
        [Parameter] public string RelatedBusiness { get; set; }
        private RenderFragment? _editableField;
        private SDKEntityField _entityReference;
        
        private void InitField()
        {
            Type? type = Context.GetType().GetProperty(Property)?.PropertyType;
            string typeName = Context.GetType().GetProperty(Property)?.PropertyType.Name ?? "";
            var access = Expression.Property(Expression.Constant(Context, Context.GetType()), Property);
            var lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(type), access);
            switch (typeName)
            {
                case "String":
                    _editableField = builder =>
                    {
                        builder.OpenComponent(0, typeof(SDKCharField));
                        builder.AddAttribute(1, "ValueExpression", lambda);
                        builder.AddAttribute(2, "Value",
                            (string)Context.GetType().GetProperty(Property)?.GetValue(Context));
                        builder.AddAttribute(3, "ValueChanged", new Action<string>(value => OnChange(value)));
                        builder.CloseComponent();
                    };
                    break;
                case "Int64":
                    _editableField = builder =>
                    {
                        builder.OpenComponent(0, typeof(SDKIntegerField<long>));
                        builder.AddAttribute(1, "ValueExpression", lambda);
                        builder.AddAttribute(2, "Value", Context.GetType().GetProperty(Property)?.GetValue(Context));
                        builder.AddAttribute(3, "ValueChanged", new Action<long>(value => OnChange(value)));
                        builder.CloseComponent();
                    };
                    break;
                case "Int32":
                    _editableField = builder =>
                    {
                        builder.OpenComponent(0, typeof(SDKIntegerField<int>));
                        builder.AddAttribute(1, "ValueExpression", lambda);
                        builder.AddAttribute(2, "Value", Context.GetType().GetProperty(Property)?.GetValue(Context));
                        builder.AddAttribute(3, "ValueChanged", new Action<int>(value => OnChange(value)));
                        builder.CloseComponent();
                    };
                    break;
                case "Int16":
                    _editableField = builder =>
                    {
                        builder.OpenComponent(0, typeof(SDKIntegerField<>).MakeGenericType(type));
                        builder.AddAttribute(1, "ValueExpression", lambda);
                        builder.AddAttribute(2, "Value", Context.GetType().GetProperty(Property)?.GetValue(Context));
                        builder.AddAttribute(3, "ValueChanged", new Action<short>(value => OnChange(value)));
                        builder.CloseComponent();
                    };
                    break;
                case "Byte":
                    _editableField = builder =>
                    {
                        builder.OpenComponent(0, typeof(DxSpinEdit<>).MakeGenericType(type));
                        builder.AddAttribute(1, "ValueExpression", lambda);
                        builder.AddAttribute(2, "Value", Context.GetType().GetProperty(Property)?.GetValue(Context));
                        builder.AddAttribute(3, "ValueChanged", new Action<byte>(value => OnChange(value)));
                    };
                    break;
                case "Decimal":
                    _editableField = builder =>
                    {
                        builder.OpenComponent(0, typeof(SDKIntegerField<>).MakeGenericType(type));
                        builder.AddAttribute(1, "ValueExpression", lambda);
                        builder.AddAttribute(2, "Value", Context.GetType().GetProperty(Property)?.GetValue(Context));
                        builder.AddAttribute(3, "ValueChanged", new Action<decimal>(value => OnChange(value)));
                        builder.CloseComponent();
                    };
                    break;
                case "DateTime":
                    _editableField = builder =>
                    {
                        builder.OpenComponent(0, typeof(SDKDateField));
                        builder.AddAttribute(1, "Value", Context.GetType().GetProperty(Property)?.GetValue(Context));
                        builder.AddAttribute(2, "ValueChanged", new Action<DateTime>(value => OnChange(value)));
                        builder.CloseComponent();
                    };
                    break;
                case "TimeOnly":
                case "TimeSpan":
                    _editableField = builder =>
                    {
                        builder.OpenComponent(0, typeof(DxTimeEdit<>).MakeGenericType(type));
                        builder.AddAttribute(1, "TimeExpression", lambda);
                        builder.AddAttribute(2, "Time", Context.GetType().GetProperty(Property)?.GetValue(Context));
                        builder.AddAttribute(3, "TimeChanged", new Action<DateTime>(value => OnChange(value)));
                        builder.CloseComponent();
                    };
                    break;
                case "DateOnly":
                    _editableField = builder =>
                    {
                        builder.OpenComponent(0, typeof(SDKDateTimeField<>).MakeGenericType(type));
                        builder.AddAttribute(2, "Value", Context.GetType().GetProperty(Property)?.GetValue(Context));
                        builder.AddAttribute(3, "ValueChanged", new Action<DateTime>(value => OnChange(value)));
                        builder.CloseComponent();
                    };
                    break;
                case "Boolean":
                    Type componentType = typeof(SDKBooleanField);
                    if (type == typeof(bool?))
                    {
                        componentType = typeof(SDKBooleanNullableField);
                    }
                    _editableField = builder =>
                    {
                        builder.OpenComponent(0, componentType);
                        builder.AddAttribute(1, "ValueExpression", lambda);
                        builder.AddAttribute(2, "Value", Context.GetType().GetProperty(Property)?.GetValue(Context));
                        builder.AddAttribute(3, "ValueChanged", new Action<bool>(value => OnChange(value)));
                        builder.CloseComponent();
                    };
                    break;
                case "EntityTextField":
                    _editableField = builder =>
                    {
                        builder.OpenComponent(0, typeof(SDKTextField));
                        builder.AddAttribute(1, "ValueExpression", lambda);
                        builder.AddAttribute(2, "Value",
                            (string)Context.GetType().GetProperty(Property)?.GetValue(Context));
                        builder.AddAttribute(3, "ValueChanged", new Action<string>(value => OnChange(value)));
                    };
                    break;
                default:
                    var bannedTypes = new List<Type>() { typeof(string), typeof(byte[]) };
                    if (type.IsClass && !type.IsPrimitive && !type.IsEnum && !bannedTypes.Contains(type) && !string.IsNullOrEmpty(RelatedBusiness))
                    {
                        _editableField = builder =>
                        {
                            builder.OpenComponent(0, typeof(SDKEntityField));
                            builder.AddAttribute(1, "RelatedBusiness", RelatedBusiness);
                            builder.AddAttribute(2, "BaseObj", Context);
                            builder.AddAttribute(3, "FieldName", Property);
                            builder.AddAttribute(4, "OnChange", new Action(() => OnChangeEntity()));
                            builder.AddComponentReferenceCapture(5, (element) => _entityReference = (SDKEntityField)element);
                            builder.CloseComponent();
                        };
                    }else if (type.IsEnum)
                    {
                        Type actionType = typeof(Action<>).MakeGenericType(type);
                        Delegate actionDelegate = Delegate.CreateDelegate(actionType, this, "OnChange", false, false);
                        var action = (Action<object>)actionDelegate;
                        _editableField = builder =>
                        {
                            builder.OpenComponent(0, typeof(SDKSelectField<>).MakeGenericType(type));
                            builder.AddAttribute(1, "ValueExpression", lambda);
                            builder.AddAttribute(2, "Value", Context.GetType().GetProperty(Property)?.GetValue(Context));
                            builder.AddAttribute(3, "ValueChanged", Delegate.CreateDelegate(actionType, this, "OnChange", false, false));
                            builder.CloseComponent();
                        };
                    }
                    else
                    {
                        _editableField = builder =>
                        {
                            builder.OpenComponent(0, typeof(SDKCharField));
                            builder.AddAttribute(1, "ValueExpression", lambda);
                            builder.AddAttribute(2, "Value", (string)Context.GetType().GetProperty(Property)?.GetValue(Context));
                            builder.AddAttribute(3, "ValueChanged", new Action<string>(value => OnChange(value)));
                            builder.CloseComponent();
                        };
                    }
                    break;
            }
        }
        
        private void OnChange(dynamic value)
        {
            if(OnChangeColumn != null)
            {
                OnChangeColumn(Context, value);
            }
            Context.GetType().GetProperty(Property)?.SetValue(Context, value);
            StateHasChanged();
        }
        
        private void OnChangeEntity()
        {
            dynamic value = _entityReference.GetItemsSelected().FirstOrDefault();
            if(OnChangeColumn != null)
            {
                OnChangeColumn(Context, value);
            }
            Context.GetType().GetProperty(Property)?.SetValue(Context, value);
            string rowidProp = "Rowid"+Property;
            Context.GetType().GetProperty(rowidProp)?.SetValue(Context, value?.Rowid);
        }
        
        protected override async Task OnParametersSetAsync()
        {
            if (IsEditable){
                InitField();
            }
            await base.OnParametersSetAsync().ConfigureAwait(true);
        }

        private dynamic GetValueColumn()
        {
            object val = Context.GetType().GetProperty(Property)?.GetValue(Context);
            if(val == null){
                return "";
            }
            return val;
        }
    }
}
