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
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Frontend.Application;
using Siesa.SDK.Frontend.Components.Fields;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Frontend.Components.FormManager.Fields;
using Siesa.SDK.Frontend.Extension;

namespace Siesa.SDK.Frontend.Components.FormManager.ViewModels
{
    public abstract class FormView : ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }

        [Parameter] 
        public bool IsSubpanel { get; set; }
        [Inject] public  Radzen.DialogService dialogService { get; set; }

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }
        [Inject] public NavigationService NavigationService { get; set; }
        [Inject] public SDKNotificationService NotificationService { get; set; }

        [Inject] protected IAuthenticationService AuthenticationService { get; set; }

        [Inject] public SDKGlobalLoaderService GlobalLoaderService { get; set; }

        protected FormViewModel FormViewModel { get; set; } = new FormViewModel();

        public List<Panel> Panels {get { return FormViewModel.Panels; } }

        public Boolean Loading = true;
        public bool Saving = false;
        public bool SavingFile { get; set; } = false;
        public String ErrorMsg = "";
        public List<string> ErrorList = new List<string>();
        [Parameter]
        public string FormID { get; set; } = Guid.NewGuid().ToString();
        protected ValidationMessageStore _messageStore;
        public EditContext EditFormContext;
        [Parameter]
        public string ViewdefName { get; set; }

        [Parameter]
        public string DefaultViewdefName { get; set; }

        [Parameter] 
        public DynamicViewType ViewContext { get; set; } = DynamicViewType.Create;
        [Parameter]
        public bool ShowTitle { get; set; } = true;
        [Parameter]
        public bool ShowButtons { get; set; } = true;
        [Parameter] 
        public bool ShowCancelButton {get; set;} = true;
        [Parameter] 
        public bool ShowSaveButton {get; set;} = true;

        [Parameter]
        public Action<object> OnSave {get; set;} = null;

        [Parameter]
        public Action OnCancel {get; set;} = null;

        [Parameter]
        public List<string> ParentBaseObj { get; set; }

        public int CountUnicErrors = 0;

        private string _viewdefName = "";

        [Inject]
        public IBackendRouterService BackendRouterService { get; set; }
        [Inject] 
        public IFeaturePermissionService FeaturePermissionService { get; set; }

        [Inject]
        public IResourceManager ResourceManager { get; set; }

        protected bool CanCreate;
        protected bool CanEdit;
        protected bool CanDelete;
        protected bool CanDetail;
        protected bool CanList;
        public bool FormHasErrors = false;

        public Dictionary<string, FileField> FileFields = new Dictionary<string, FileField>();
        public SDKFileUploadDTO DataAttatchmentDetail { get; set; }
        protected virtual async Task CheckPermissions()
        {
            if (FeaturePermissionService != null && !String.IsNullOrEmpty(BusinessName))
            {
                try
                {
                    CanList = FeaturePermissionService.CheckUserActionPermission(BusinessName, 4, AuthenticationService);
                    CanCreate = FeaturePermissionService.CheckUserActionPermission(BusinessName, 1, AuthenticationService);
                    CanEdit = FeaturePermissionService.CheckUserActionPermission(BusinessName, 2, AuthenticationService);
                    CanDelete = FeaturePermissionService.CheckUserActionPermission(BusinessName, 3, AuthenticationService);
                    CanDetail = FeaturePermissionService.CheckUserActionPermission(BusinessName, 5, AuthenticationService);
                }
                catch (System.Exception)
                {
                }
            }
        }
        
        private string GetViewdef(string businessName)
        {
            if (String.IsNullOrEmpty(ViewdefName))
            {
                _viewdefName = DefaultViewdefName;
            }else{
                _viewdefName = ViewdefName;
            }

            var data = BackendRouterService.GetViewdef(businessName, _viewdefName);
            if (String.IsNullOrEmpty(data) && _viewdefName != DefaultViewdefName)
            {
                data = BackendRouterService.GetViewdef(businessName, DefaultViewdefName);
            }
            if(String.IsNullOrEmpty(data)){
                _viewdefName = "default";
                data = BackendRouterService.GetViewdef(businessName, _viewdefName);
            }
            return data;
        }

        private void setViewContext(List<Panel> panels, DynamicViewType viewType)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if(String.IsNullOrEmpty(panels[i].ResourceTag))
                {
                    if(String.IsNullOrEmpty(panels[i].ResourceTag)){
                        panels[i].ResourceTag = $"{BusinessName}.Viewdef.{_viewdefName}.Panel.{panels[i].Name}";
                    }
                }
                
                for (int j = 0; j < panels[i].Fields.Count; j++)
                {
                    if(viewType == DynamicViewType.Detail && String.IsNullOrEmpty(panels[i].Fields[j].ViewContext))
                    {
                        panels[i].Fields[j].ViewContext = "DetailView";
                    }
                    
                    panels[i].Fields[j].GetFieldObj(BusinessObj);
                }
                if (panels[i].SubViewdef != null && panels[i].SubViewdef.Panels.Count > 0)
                {
                    setViewContext(panels[i].SubViewdef.Panels, viewType);
                }
            }
        }

        public void Refresh(bool Reload = false){
            EvaluateDynamicAttributes(null);
            try
            {
                StateHasChanged();
            }
            catch (System.Exception)
            {
                _ = InvokeAsync(() => StateHasChanged());
            }
        }
        protected virtual async Task InitView(string bName = null)
        {
            Loading = true;
            if (bName == null)
            {
                bName = BusinessName;
            }
            await CheckPermissions();
            var metadata = GetViewdef(bName);
            if (metadata == null || metadata == "")
            {
                //string ErrorTag = await ResourceManager.GetResource("Custom.Formview.NotDefinition", AuthenticationService.GetRoiwdCulture());
                ErrorMsg = $"Custom.Generic.ViewdefNotFound";
                ErrorList.Add($"Custom.Generic.ViewdefNotFound");
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
            try
            {
                setViewContext(FormViewModel.Panels, ViewContext);
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error");
            }
            if(FormViewModel.Relationships != null && FormViewModel.Relationships.Count > 0)
            {
                foreach (var relationship in FormViewModel.Relationships)
                {
                    if(String.IsNullOrEmpty(relationship.ResourceTag))
                    {
                        relationship.ResourceTag = $"{BusinessName}.Relationship.{relationship.Name}";
                    }
                }
            }
            Loading = false;
            EditFormContext = new EditContext(BusinessObj);
            EditFormContext.OnFieldChanged += EditContext_OnFieldChanged;
            _messageStore = new ValidationMessageStore(EditFormContext);
            EditFormContext.OnValidationRequested += (s, e) => _messageStore.Clear();
            EvaluateDynamicAttributes(null);
            BusinessObj.ParentComponent = this;
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
            //await InitView();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            bool changeViewContext = parameters.DidParameterChange(nameof(ViewContext), ViewContext);
            bool changeBusinessName = parameters.DidParameterChange(nameof(BusinessName), BusinessName);

            await base.SetParametersAsync(parameters);

            if(changeViewContext || changeBusinessName)
            {
                Loading = false;
                ErrorMsg = "";
                ErrorList = new List<string>();
                await InitView();
            }
        }
        public void OnError(SDKUploadErrorEventArgsDTO args){
			var error = args.Message;
			SavingFile = false;
		}

        public async Task<int> savingAttachment(FileField fileField, int rowid = 0){
            SavingFile = true;
            await fileField.Upload();
            var horaInicio = DateTime.Now.Minute;
            while(SavingFile){
                await Task.Delay(100);
                if (DateTime.Now.Minute - horaInicio >= 2){
                    throw new Exception("Timeout");
                }
            }
            if(DataAttatchmentDetail != null){
                var result = await BusinessObj.SaveAttachmentDetail(DataAttatchmentDetail, rowid);
                return result;
            }
            return 0;
        }

        public void OnComplete(SDKUploadCompleteEventArgsDTO args){
            var response  = JsonConvert.DeserializeObject<dynamic>(args.RawResponse);
			var data = response.data;
            if(data != null){
                DataAttatchmentDetail = JsonConvert.DeserializeObject<SDKFileUploadDTO>(data.ToString());
            }
			SavingFile = false;
        }
        private async Task SaveBusiness()
        {
            Saving = true;
            //var id = await BusinessObj.SaveAsync();
            if(CountUnicErrors>0){
                GlobalLoaderService.Hide();
                Saving = false;
                var existeUniqueIndexValidation = NotificationService.Messages.Where(x => x.Summary == "Custom.Generic.UniqueIndexValidation").Any();
                if(!existeUniqueIndexValidation){
                    NotificationService.ShowError("Custom.Generic.UniqueIndexValidation");
                }
                return;
            }
            GlobalLoaderService.Show();
            if(FileFields.Count>0){
                foreach (var item in FileFields)
                {
                    var fileField = item.Value;
                    var rowidAttatchment = 0;
                    var field = item.Key;
                    dynamic property = BusinessObj.BaseObj.GetType().GetProperty(field).GetValue(BusinessObj.BaseObj);
                    if(property != null){ 
                        var rowidProp = property.PropertyType.GetProperty("Rowid").GetValue(property.GetValue(BusinessObj.BaseObj));
                        if(rowidProp != null)
                            rowidAttatchment = await savingAttachment(fileField, (int)rowidProp);
                    }else{
                        rowidAttatchment = await savingAttachment(fileField);
                    }
                    if(rowidAttatchment > 0){
                        var AttatchmentDetail = Activator.CreateInstance(BusinessObj.BaseObj.GetType().GetProperty(field).PropertyType);
                        AttatchmentDetail.GetType().GetProperty("Rowid").SetValue(AttatchmentDetail, rowidAttatchment);
                        BusinessObj.BaseObj.GetType().GetProperty(field).SetValue(BusinessObj.BaseObj, AttatchmentDetail);
                    }
                }
            }
            dynamic result = null;
            try{
                result = await BusinessObj.ValidateAndSaveAsync();
            }catch(Exception ex){
                GlobalLoaderService.Hide();
                Saving = false;
                ErrorMsg = ex.Message;
                ErrorList.Add("Exception: "+ex.Message);
                return;
            }

            GlobalLoaderService.Hide();
            Saving = false;
            ErrorMsg = string.Empty;
            if (result.Errors.Count > 0)
            {
                //ErrorMsg = "<ul>";
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
                    // ErrorMsg += $"<li>";
                    // ErrorMsg += !string.IsNullOrWhiteSpace(error.Attribute) ?  $"{error.Attribute} - " : string.Empty;
                    // string ErrorTag = await ResourceManager.GetResource(error.Message, AuthenticationService.GetRoiwdCulture());
                    // ErrorMsg += ErrorTag;//error.Message.Replace("\n", "<br />");
                    // ErrorMsg += $"</li>";

                    ErrorList.Add("Exception: "+error.Message);
                }
                //ErrorMsg += "</ul>";
                EditFormContext.NotifyValidationStateChanged();


                return;
            }
            var id = result.Rowid;
            if(IsSubpanel){
                dialogService.Close(id);
                if(OnSave != null){
                    OnSave(id);
                }
                StateHasChanged();
            }else{
                NavigationService.RemoveCurrentItem();
                NavManager.NavigateTo($"{BusinessName}/detail/{id}/");
            }
        }

        public async Task CancelButton(){
            dialogService.Close(false);
            if(OnCancel != null){
                OnCancel();
            }
        }

        protected async Task HandleValidSubmit()
        {
            FormHasErrors = false;
            ErrorMsg = "";
            ErrorList.Clear();
            await SaveBusiness();
        }
        protected void HandleInvalidSubmit()
        {
            //ErrorMsg = @"Form data is invalid";
            FormHasErrors = true;
            NotificationService.ShowError("Custom.Generic.FormError");
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
        
        protected string GetSaveBtnIcon()
        {
            return Saving ? "fas fa-spinner fa-pulse" : "fa-solid fa-floppy-disk";
        }

    }
}
