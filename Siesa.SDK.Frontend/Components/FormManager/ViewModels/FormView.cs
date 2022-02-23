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

        protected FormViewModel FormViewModel { get; set; } = new FormViewModel();

        protected List<Panel> Panels {get { return FormViewModel.Panels; } }

        public Boolean Loading = true;

        public String ErrorMsg = "";
        public string FormID { get; set; } = Guid.NewGuid().ToString();
        protected ValidationMessageStore _messageStore;
        protected EditContext EditFormContext;

        public string ViewdefName { get; set; }

        protected void InitView(string bName = null)
        {
            Loading = true;
            if (bName == null)
            {
                bName = BusinessName;
            }
            var metadata = BusinessManagerFrontend.Instance.GetViewdef(bName, ViewdefName);
            if (metadata == null || metadata == "")
            {
                ErrorMsg = $"No hay definición para la vista {ViewdefName}";
            }
            else
            {
                try
                {
                    FormViewModel = JsonConvert.DeserializeObject<FormViewModel>(metadata);
                }
                catch (System.Exception)
                {
                    //Soporte a viewdefs anteriores
                    var panels = JsonConvert.DeserializeObject<List<Panel>>(metadata);
                    FormViewModel.Panels = panels;
                }
            }
            Loading = false;
            EditFormContext = new EditContext(BusinessObj);
            EditFormContext.OnFieldChanged += EditContext_OnFieldChanged;
            _messageStore = new ValidationMessageStore(EditFormContext);
            EditFormContext.OnValidationRequested += (s, e) => _messageStore.Clear();
            EvaluateDynamicAttributes(null);
            StateHasChanged();
        }

        private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            _messageStore.Clear(e.FieldIdentifier);
            EvaluateDynamicAttributes(e);
        }

        private void EvaluateDynamicAttributes(FieldChangedEventArgs e)
        {
            string code = "";
            foreach (var item in Panels.Select((value, i) => (value, i)))
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
try {{ Panels[{panel_index}].Fields[{field_index}].Hidden = !({(string)attr.Value}); }} catch (Exception ex) {{ throw;  }}";
                                /*_ = Task.Run(async () =>
                                {
                                    var result = (bool)await Evaluator.EvaluateCode((string)attr.Value, BusinessObj);
                                    field.Hidden = !result;
                                    _ = InvokeAsync(() => StateHasChanged());
                                });*/
                                break;
                            case "sdk-hide":
                                code += @$"
try {{ Panels[{panel_index}].Fields[{field_index}].Hidden = ({(string)attr.Value}); }} catch (Exception ex) {{ throw;  }}";
                                /*_ = Task.Run(async () =>
                                {
                                    var result = (bool)await Evaluator.EvaluateCode((string)attr.Value, BusinessObj);
                                    field.Hidden = result;
                                    _ = InvokeAsync(() => StateHasChanged());
                                });*/
                                break;
                            case "sdk-required":
                                code += @$"
try {{ Panels[{panel_index}].Fields[{field_index}].Required = ({(string)attr.Value}); }} catch (Exception ex) {{ throw;  }}";
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
try {{ Panels[{panel_index}].Fields[{field_index}].Disabled = ({(string)attr.Value}); }} catch (Exception ex) {{ throw; }}";
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
                     BusinessObj.Panels = Panels;
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
            //var id = await BusinessObj.SaveAsync();
            var result = await BusinessObj.ValidateAndSaveAsync();
            Loading = false;
            ErrorMsg = string.Empty;
            if (result.Errors.Count > 0)
            {
                ErrorMsg = "<ul>";
                Type editFormCurrentType = EditFormContext.Model.GetType();
                foreach (var error in result.Errors)
                {
                    FieldIdentifier fieldIdentifier;
                    bool fieldInContext = false;
                    //check if attribute is in Model
                    // if(editFormCurrentType.GetProperty(error.Attribute) != null)
                    // {
                    //     fieldIdentifier = new FieldIdentifier(EditFormContext.Model, error.Attribute);
                    //     fieldInContext = true;
                    // }
                    // else if(((string)error.Attribute).Split('.').Count() > 1)
                    // {
                    //     var attr = ((string)error.Attribute).Split('.');
                    //     foreach(string item in attr)
                    //     {
                    //         if(editFormCurrentType.GetProperty(item) != null)
                    //         {
                    //             editFormCurrentType = editFormCurrentType.GetProperty(item).PropertyType;
                    //         }

                    //     }
                    //     fieldIdentifier = new FieldIdentifier(EditFormContext.Model, $"BaseObj.{error.Attribute}");
                    //     fieldInContext = true;
                    // }
                    // if(fieldInContext)
                    // {
                    //     _messageStore.Add(fieldIdentifier, (string)error.Message);
                    // }

                    fieldIdentifier = new FieldIdentifier(EditFormContext.Model, error.Attribute);
                    _messageStore.Add(fieldIdentifier, (string)error.Message);
                    
                    


                    ErrorMsg += $"<li>";
                    ErrorMsg += !string.IsNullOrWhiteSpace(error.Attribute) ?  $"{error.Attribute} - " : string.Empty;
                    ErrorMsg += error.Message.Replace("\n", "<br />");
                    ErrorMsg += $"</li>";
                }
                ErrorMsg += "</ul>";
                EditFormContext.NotifyValidationStateChanged();


                return;
            }
            var id = result.Rowid;
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

        public async Task OnClickCustomButton(Button button)
        {
            if (!string.IsNullOrEmpty(button.Href))
            {
                if (button.Target == "_blank")
                {
                    _ = JSRuntime.InvokeVoidAsync("window.open", button.Href, "_blank");
                }
                else
                {
                    NavManager.NavigateTo(button.Href);
                }


            }
            else if (!string.IsNullOrEmpty(button.Action))
            {
                await Evaluator.EvaluateCode(button.Action, BusinessObj);
                StateHasChanged();
            }
        }
    }
}
