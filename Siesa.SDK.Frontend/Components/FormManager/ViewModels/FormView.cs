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
using Siesa.SDK.Frontend.Utils;

namespace Siesa.SDK.Frontend.Components.FormManager.ViewModels
{
    public abstract class FormView : ComponentBase
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

        protected EditContext EditFormContext;

        public string ViewdefName { get; set; }

        protected void InitView(string bName = null)
        {
            Loading = true;
            if (bName == null)
            {
                bName = BusinessName;
            }
            var metadata = BusinessManager.Instance.GetViewdef(bName, ViewdefName);
            if (metadata == null || metadata == "")
            {
                ErrorMsg = $"No hay definición para la vista {ViewdefName}";
            }
            else
            {
                Paneles = JsonConvert.DeserializeObject<List<Panel>>(metadata);
            }
            Loading = false;
            EditFormContext = new EditContext(BusinessObj);
            EditFormContext.OnFieldChanged += EditContext_OnFieldChanged;
            EvaluateDynamicAttributes(null);
            StateHasChanged();
        }

        private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            Console.WriteLine("algo cambió en el form");
            EvaluateDynamicAttributes(e);
        }

        private void EvaluateDynamicAttributes(FieldChangedEventArgs e)
        {
            string code = "";
            foreach (var item in Paneles.Select((value, i) => (value, i)))
            {
                var panel_index = item.i;
                var panel = item.value;
                if (panel.Fields == null)
                {
                    continue;
                }

                foreach (var fieldItem in panel.Fields.Select((value, i) => (value, i)))
                {
                    var field_index = fieldItem.i;
                    var field = fieldItem.value;
                    if(field.CustomAttributes == null)
                    {
                        continue;
                    }

                    var fieldCustomAttr = field.CustomAttributes.Where(x => x.Key.StartsWith("sdk-") && x.Key != "sdk-change");
                    foreach (var attr in fieldCustomAttr)
                    {
                        //hacer casteo a enum y refactorizar
                        switch (attr.Key)
                        {
                            case "sdk-show":
                                code += @$"
try {{ Paneles[{panel_index}].Fields[{field_index}].Hidden = !({(string)attr.Value}); }} catch (Exception ex) {{ throw;  }}";
                                /*_ = Task.Run(async () =>
                                {
                                    var result = (bool)await Evaluator.EvaluateCode((string)attr.Value, BusinessObj);
                                    field.Hidden = !result;
                                    _ = InvokeAsync(() => StateHasChanged());
                                });*/
                                break;
                            case "sdk-hide":
                                code += @$"
try {{ Paneles[{panel_index}].Fields[{field_index}].Hidden = ({(string)attr.Value}); }} catch (Exception ex) {{ throw;  }}";
                                /*_ = Task.Run(async () =>
                                {
                                    var result = (bool)await Evaluator.EvaluateCode((string)attr.Value, BusinessObj);
                                    field.Hidden = result;
                                    _ = InvokeAsync(() => StateHasChanged());
                                });*/
                                break;
                            case "sdk-required":
                                code += @$"
try {{ Paneles[{panel_index}].Fields[{field_index}].Required = ({(string)attr.Value}); }} catch (Exception ex) {{ throw;  }}";
                                /*_ = Task.Run(async () =>
                                {
                                    var result = (bool)await Evaluator.EvaluateCode((string)attr.Value, BusinessObj);
                                    field.Required = result;
                                    _ = InvokeAsync(() => StateHasChanged());
                                });*/
                                break;
                            case "sdk-readonly":
                            case "sdk-disabled":
                                code += @$"
try {{ Paneles[{panel_index}].Fields[{field_index}].Disabled = ({(string)attr.Value}); }} catch (Exception ex) {{ throw; }}";
                                /*_ = Task.Run(async () =>
                                {
                                    var result = (bool)await Evaluator.EvaluateCode((string)attr.Value, BusinessObj);
                                    field.Disabled = result;
                                    _ = InvokeAsync(() => StateHasChanged());
                                });*/
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            //Console.WriteLine(code);
            if(code != null & code != "")
            {
                _ = Task.Run(async () =>
                 {
                     BusinessObj.Paneles = Paneles;
                     await Evaluator.EvaluateCode(code, BusinessObj);
                     _ = InvokeAsync(() => StateHasChanged());
                 });
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

        protected async Task HandleValidSubmit()
        {
            ErrorMsg = @"Form data is valid";
            await SaveBusiness();
        }
        protected void HandleInvalidSubmit()
        {
            ErrorMsg = @"Form data is invalid";
        }
        protected void GoToList()
        {
            NavManager.NavigateTo($"{BusinessName}/");
        }
    }
}
