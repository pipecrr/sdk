using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Siesa.SDK.Frontend.Utils;

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

        public bool IsUnique { get; set; }
        private RenderFragment? _fieldValidationTemplate;

        private string OnChange { get; set; }

        [CascadingParameter] EditContext EditFormContext { get; set; }

        protected void RefreshMe()
        {
           InvokeAsync(StateHasChanged);
        }

        protected async Task Init()
        {
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
            if (FieldOpt.Required)
            {
                IsRequired = true;
            }
            //TODO: Optimizar, el parametro deberia entrar por parametro y no consultar la entidad por cada campo
            var entityAttributes = BindModel.GetType().GetCustomAttributes();
            foreach (var attr in entityAttributes)
            {
                switch (attr)
                {
                    case IndexAttribute _:
                        if (((IndexAttribute)attr).IsUnique)
                        {
                            var propNames = ((IndexAttribute)attr).PropertyNames;
                            foreach (var propName in propNames)
                            {
                                if (propName == FieldName)
                                {
                                    IsUnique = true;
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
            OnChange = (string)FieldOpt.CustomAttributes?.Where(x => x.Key == "sdk-change").FirstOrDefault().Value;

        }

        protected override async Task OnInitializedAsync()
        {
            
            await base.OnInitializedAsync();
            //await Init();
        }

        public void SetValue(TProperty value)
        {
            var setValue = true;
            
            if (IsUnique)
            {
                Console.WriteLine($"El campo {FieldName} es único y debe revisar el valor {value}");
                //setValue = CheckUnique(value);
            }

            if (setValue)
            {
                BindValue = value;
                if (OnChange != null && OnChange != "") {
                    _ = Task.Run(async () =>
                    {
                        await Evaluator.EvaluateCode(OnChange, EditFormContext.Model);
                        _ = InvokeAsync(() => StateHasChanged());
                    });
                }
            }
            else
            {
                //MuestreError();
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

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            await Init();
        }

    }
}
