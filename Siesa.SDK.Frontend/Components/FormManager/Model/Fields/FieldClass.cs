using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Forms;

namespace Siesa.SDK.Frontend.Components.FormManager.Model.Fields
{
    public class FieldClass<TProperty> : ComponentBase 
    {         
        public TProperty BindValue { get {
                return (TProperty)BindProperty?.GetValue(BindModel, null);
            }
            set {
                BindProperty.SetValue(BindModel, value);
            }
        }
        [Parameter] public FieldOptions FieldOpt { get; set; }

        [Parameter] public object BindModel { get; set; }

        public PropertyInfo BindProperty { get; set; }

        [Parameter] public TProperty Text { get; set; }

        //[Parameter] public Func<int> RefresParent { get; set; }

        [Parameter] public List<string> EscucharA { get; set; } = new List<string>();

        [Inject] protected IJSRuntime jsRuntime { get; set; }

        [Parameter] public EventCallback RefreshParent { get; set; }

        private object FinalBindModel { get; set; }

        [Parameter] public string FieldName { get; set; }

        public bool IsRequired { get; set; }
        public int MaxLength { get; set; }

        private RenderFragment? _fieldValidationTemplate;

        protected void RefreshMe()
        {
           InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            BindProperty = BindModel.GetType().GetProperty(FieldName);
            var customAttr = BindProperty.GetCustomAttributes();
            foreach (var attr in customAttr)
            {
                switch (attr)
                {
                    case RequiredAttribute _:
                        IsRequired = true;
                        break;
                    case MaxLengthAttribute _:
                        MaxLength = ((MaxLengthAttribute)attr).Length;
                        break;
                }
            }            
        }

        public RenderFragment? FieldValidationTemplate
        {
            get
            {

                return _fieldValidationTemplate != null ? _fieldValidationTemplate : builder =>
                {
                    var access = Expression.Property(Expression.Constant(BindModel, BindModel.GetType()), FieldName);
                    var lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(typeof(TProperty)), access);

                    builder.OpenComponent(0, typeof(ValidationMessage<>).MakeGenericType(typeof(TProperty)));
                    builder.AddAttribute(1, "For", lambda);
                    builder.CloseComponent();
                };
            }
        }

    }
}
