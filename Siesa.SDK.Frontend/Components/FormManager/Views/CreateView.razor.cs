using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Microsoft.JSInterop;
using Siesa.SDK.Business;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Linq;
namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class CreateView : ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }

        protected List<Panel> Paneles = new List<Panel>();

        public Boolean Loading = true;

        public String ErrorMsg = "";
        public string FormID { get; set; } = Guid.NewGuid().ToString();

        private EditContext EditFormContext;

        protected void InitView(string bName = null)
        {
            Loading = true;
            if (bName == null)
            {
                bName = BusinessName;
            }
            var metadata = BusinessManager.Instance.GetViewdef(bName, "create");
            if (metadata == null || metadata == "")
            {
                ErrorMsg = "No hay definición para la vista de creación";
            }
            else
            {
                Paneles = JsonConvert.DeserializeObject<List<Panel>>(metadata);
            }
            Loading = false;
            EditFormContext = new EditContext(BusinessObj);
            EditFormContext.OnFieldChanged += EditContext_OnFieldChanged;
            StateHasChanged();
        }

        private async Task<object> EvaluateCode(string code, object globals)
        {
            object result;
            try
            {
                result = await CSharpScript.EvaluateAsync(code, globals: globals);
            }
            catch (Exception)
            {
                Console.WriteLine("error, eval");
                return null;
            }

            return result;
        }

        private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            Console.WriteLine("algo cambió en el form");
            foreach (var item in Paneles)
            {
                if (item.Fields == null)
                {
                    continue;
                }

                foreach (var field in item.Fields)
                {
                    if(field.CustomAtributes == null)
                    {
                        continue;
                    }

                    var fieldCustomAttr = field.CustomAtributes.Where(x => x.Key.StartsWith("sdk-"));
                    foreach (var attr in fieldCustomAttr)
                    {
                        //hacer casteo a enum y refactorizar
                        switch (attr.Key)
                        {
                            case "sdk-change":
                                if (e.FieldIdentifier.FieldName == field.PropertyName) //TODO: Arreglar error de campos con el mismo nombre y diferente modelo
                                {
                                    _ = Task.Run(async () =>
                                    {
                                        var result = await EvaluateCode((string)attr.Value, BusinessObj);
  
                                        _ = InvokeAsync(() => StateHasChanged());
                                    });
                                }
                                break;
                            case "sdk-show":
                                _ = Task.Run(async () =>
                                {
                                    var result = (bool)await EvaluateCode((string)attr.Value, BusinessObj);
                                    field.Hidden = !result;
                                    _ = InvokeAsync(() => StateHasChanged());
                                });
                                break;
                            case "sdk-hide":
                                _ = Task.Run(async () =>
                                {
                                    var result = (bool)await EvaluateCode((string)attr.Value, BusinessObj);
                                    field.Hidden = result;
                                    _ = InvokeAsync(() => StateHasChanged());
                                });
                                break;
                            case "sdk-required":
                                _ = Task.Run(async () =>
                                {
                                    var result = (bool)await EvaluateCode((string)attr.Value, BusinessObj);
                                    field.Required = result;
                                    _ = InvokeAsync(() => StateHasChanged());
                                });
                                break;
                            default:
                                break;
                        }
                    }
                }
                
            }
        }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //InitView();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (parameters.TryGetValue<string>(nameof(BusinessName), out var value))
            {
                if (value != null)
                {
                    Loading = false;
                    ErrorMsg = "";
                    InitView(value);
                }
            }
        }

        private async Task SaveBusiness()
        {
            Loading = true;
            StateHasChanged();
            var id = await BusinessObj.SaveAsync();
            Loading = false;
            NavManager.NavigateTo($"{BusinessName}/detail/{id}/");
        }

        async Task HandleValidSubmit()
        {
            ErrorMsg = @"Form data is valid";
            await SaveBusiness();
        }
        void HandleInvalidSubmit()
        {
            ErrorMsg = @"Form data is invalid";
        }
    }
}
