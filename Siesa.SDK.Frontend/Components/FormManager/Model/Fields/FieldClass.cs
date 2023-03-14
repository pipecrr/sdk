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
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Frontend.Components.Visualization;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Shared.DataAnnotations;

namespace Siesa.SDK.Frontend.Components.FormManager.Model.Fields
{
    public class FieldClass<TProperty> : ComponentBase
    {

        public TProperty BindValue
        {
            get
            {
                var modelValue = BindProperty?.GetValue(BindModel, null);
                
                if (modelValue == null)
                {
                    return default(TProperty);
                }

                return (TProperty)modelValue;
            }
            set
            {
                BindProperty.SetValue(BindModel, value);
            }
        }
        [Parameter] public FieldOptions FieldOpt { get; set; }

        [Parameter] public object BindModel { get; set; }

        public PropertyInfo BindProperty { get; set; }

        [Parameter] public TProperty Text { get; set; }

        [Inject] protected IJSRuntime jsRuntime { get; set; }
        [Inject] private IBackendRouterService _backendRouterService { get; set; }

        [Inject] private IAuthenticationService _authenticationService { get; set; }

        [Inject] private SDKNotificationService _NotificationService { get; set; }

        [Parameter] public string FieldName { get; set; }
        [Parameter] public bool ValidateField { get; set; } = true;

        private IJSObjectReference _jsModule;

        public bool IsRequired { get; set; }
        public int MaxLength { get; set; }

        public bool IsEncrypted { get; set; }

        public bool IsUnique { get; set; }

        public bool IsNullable { get; set; }
        private RenderFragment? _fieldValidationTemplate;

        private string OnChange { get; set; }
        private bool HasError { get; set; }

        [CascadingParameter] EditContext EditFormContext { get; set; }
        [CascadingParameter] FormView formView { get; set; }

        public string ViewdefName { get 
        {
            if(formView == null)
            {
                return "";
            }
            return formView.ViewdefName;
        }}

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
                    case SDKDataEncrypt _:
                        IsEncrypted = true;
                        break;
                }
            }
            if (FieldOpt.Required)
            {
                IsRequired = true;
            }
            if (IsEncrypted)
            {
                if (BindValue != null)
                {
                    BindProperty.SetValue(BindModel, null);
                }
            }

            if(IsRequired && !ValidateField)
            {
                IsRequired = false;
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
                                if (propName == FieldName || propName == $"Rowid{FieldName}")
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
            StateHasChanged();

        }

        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
            //await Init();
        }

        public void SetValue(string fieldName, object value)
        {
            var fieldProperty = BindModel.GetType().GetProperty(fieldName);
            fieldProperty.SetValue(BindModel, value);
        }

        private async Task CheckUniqueValue()
        {
            try
            {
                var request = await _backendRouterService.GetSDKBusinessModel(this.formView.BusinessName, _authenticationService).Call("CheckUnique", this.BindModel);
                if (request.Success)
                {
                    if (request.Data == true)
                    {
                        var existeUniqueIndexValidation = _NotificationService.Messages.Where(x => x.Summary == "Custom.Generic.UniqueIndexValidation").Any();
                        if(!existeUniqueIndexValidation){
                            _NotificationService.ShowError("Custom.Generic.UniqueIndexValidation");
                        }

                        if(this.FieldOpt.CssClass == null)
                        {
                            this.FieldOpt.CssClass = "";
                        }

                        if(!this.FieldOpt.CssClass.Contains("sdk-unique-invalid"))
                        {
                            this.FieldOpt.CssClass += " sdk-unique-invalid";
                        }
                        var typeComp = typeComponent();
                        var dataAutomationId = $"{typeComp}_{this.FieldOpt.Name}";
                        await jsRuntime.InvokeVoidAsync("SetFocusToElement", dataAutomationId);
                        if(!HasError){
                            this.formView.CountUnicErrors += 1;
                            HasError = true;
                        }
                    }else{
                        if(this.FieldOpt.CssClass != null && this.FieldOpt.CssClass.Contains("sdk-unique-invalid"))
                        {
                            this.FieldOpt.CssClass = this.FieldOpt.CssClass.Replace(" sdk-unique-invalid", "");
                        }
                        this.formView.CountUnicErrors -= 1;
                        HasError = false;
                    }
                }else{
                    if(this.FieldOpt.CssClass != null && this.FieldOpt.CssClass.Contains("sdk-unique-invalid"))
                    {
                        this.FieldOpt.CssClass = this.FieldOpt.CssClass.Replace(" sdk-unique-invalid", "");
                    }
                    this.formView.CountUnicErrors -= 1;
                    HasError = false;
                }
                StateHasChanged();
            }
            catch (Exception e)
            {
                _NotificationService.ShowError(e.Message);
                if(this.FieldOpt.CssClass != null && this.FieldOpt.CssClass.Contains("sdk-unique-invalid"))
                {
                    this.FieldOpt.CssClass = this.FieldOpt.CssClass.Replace(" sdk-unique-invalid", "");
                }
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender){
            if(firstRender || _jsModule == null){                
                _jsModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Siesa.SDK.Frontend/js/utils.js");
            }
        }

        private string typeComponent(){
            string result = "";

            switch(FieldOpt.FieldType.ToString()){
                case "CharField":
                    result = "SDKCharField";
                    break;
                case "EntityField":
                    result = "SDKEntityField";
                    break;
                case "FileField":
                    result = "SDKFileField";
                    break;
            }
            return  result;
        }

        public void SetValue(TProperty value)
        {
            var setValue = true;

            if (setValue)
            {
                BindValue = value;
                if (OnChange != null && OnChange != "")
                {
                    _ = Task.Run(async () =>
                    {
                        await Evaluator.EvaluateCode(OnChange, EditFormContext.Model);
                        if (formView != null)
                        {
                            _ = InvokeAsync(() => formView.Refresh());
                        }
                    });
                }
            }
            else
            {
                //MuestreError();
            }
            if (IsUnique && ValidateField)
            {
                CheckUniqueValue();
                 //Console.WriteLine($"El campo {FieldName} es único y debe revisar el valor {value}");
            }

        }

        public RenderFragment? FieldValidationTemplate
        {
            get
            {

                return _fieldValidationTemplate != null ? _fieldValidationTemplate : builder =>
                {
                    var access = Expression.Property(Expression.Constant(BindModel, BindModel.GetType()), FieldName);
                    var lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(access.Type), access);
                    builder.OpenComponent(0, typeof(SDKValidationMessage<>).MakeGenericType(access.Type));
                    builder.AddAttribute(1, "For", lambda);
                    builder.CloseComponent();
                };
            }
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            await Init();
            StateHasChanged();
        }

    }
}
