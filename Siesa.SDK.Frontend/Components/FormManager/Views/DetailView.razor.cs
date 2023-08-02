using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Microsoft.JSInterop;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Utils;
using Radzen;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Entities;
using Siesa.Global.Enums;
using Siesa.SDK.Frontend.Components.FormManager.Fields;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DetailView : ComponentBase
    {
        [Inject] public Radzen.DialogService dialogService { get; set; }
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }

        [Parameter]
        public bool SetTopBar { get; set; } = true;

        [Parameter]
        public bool IsSubpanel { get; set; }

        [Parameter]
        public bool ShowTitle { get; set; } = true;
        [Parameter]
        public bool ShowButtons { get; set; } = true;
        [Parameter]
        public bool ShowCancelButton { get; set; } = true;

        [Parameter]
        public bool ShowDeleteButton { get; set; } = true;

        [Parameter]
        public string ViewdefName { get; set; }

        [Parameter]
        public string BLNameParentAttatchment { get; set; }

        public Boolean Loading = true;

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }

        [Inject] public NavigationService NavigationService { get; set; }
        [Inject]
        public IBackendRouterService BackendRouterService { get; set; }

        [Inject] public IFeaturePermissionService FeaturePermissionService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }

        [Inject] public SDKNotificationService NotificationService { get; set; }        
        protected FormViewModel FormViewModel { get; set; } = new FormViewModel();
        protected List<Panel> Panels { get { return FormViewModel.Panels; } }
        public List<Panel> PanelsCollapsable = new List<Panel>();
        public Boolean ModelLoaded = false;
        public String ErrorMsg = "";
        public List<string> ErrorList = new List<string>();
        protected bool CanCreate;
        protected bool CanEdit;
        protected bool CanDelete;
        protected bool CanDetail;
        protected bool CanAcess;
        public Boolean ContainAttachments = false;
        Relationship RelationshipAttachment = new Relationship();
        E00270_Attachment ParentAttachment = new E00270_Attachment();

        private bool HasExtraButtons { get; set; }
        private List<Button> ExtraButtons { get; set; }
        private Button CreateButton { get; set; }
        private Button DuplicateButton { get; set; }
        private Button EditButton { get; set; }
        private Button ListButton { get; set; }
        private Button DeleteButton { get; set; }
        private string _viewdefName;

        private void setViewContextField(FieldOptions field)
        {
            field.ViewContext = "DetailView";
            field.GetFieldObj(BusinessObj);

            if (field.Fields == null || field.Fields.Count <= 0) return;
            foreach (var item in field.Fields.Select((value, i) => (value, i)))
            {
                setViewContextField(item.value);
            }
        }
        private void setViewContext(List<Panel> panels)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if (String.IsNullOrEmpty(panels[i].ResourceTag))
                {
                    if (String.IsNullOrEmpty(panels[i].ResourceTag))
                    {
                        panels[i].ResourceTag = $"{BusinessName}.Viewdef.detail.Panel.{panels[i].Name}";
                    }
                }

                for (int j = 0; j < panels[i].Fields.Count; j++)
                {
                    setViewContextField(panels[i].Fields[j]);
                }
                if (panels[i].SubViewdef != null && panels[i].SubViewdef.Panels.Count > 0)
                {
                    setViewContext(panels[i].SubViewdef.Panels);
                }
            }

        }

        private void EvaluateDynamicAttributes()
        {
            foreach (var panelFields in Panels.Select(panel => panel.Fields))
            {
                EvaluateFields(panelFields);
            }
        }
        
        private void EvaluateFields(List<FieldOptions> panelFields)
        {
            foreach (var field in panelFields)
            {
                EvaluateFieldAttributes(field);

                if (field.Fields != null && field.Fields.Count > 0)
                {
                    EvaluateFields(field.Fields);
                }
            }
        }

        private void EvaluateFieldAttributes(FieldOptions field)
        {
            if (field.CustomAttributes == null)
            {
                return;
            }

            var fieldCustomAttr = field.CustomAttributes
                .Where(x => x.Key.StartsWith("sdk-", StringComparison.Ordinal) && x.Key != "sdk-change")
                .ToList();

            List<string> allowAttr = new List<string>(){
                "sdk-show",
                "sdk-hide",
                "sdk-required",
                "sdk-readonly",
                "sdk-disabled"
            }; //TODO: Enum

            _ = Task.Run(async () =>
            {
                bool shouldUpdate = false;

                if (fieldCustomAttr == null)
                {
                    return;
                }

                foreach (var attr in fieldCustomAttr)
                {
                    if (!allowAttr.Contains(attr.Key))
                    {
                        continue;
                    }

                    try
                    {
                        var result = (bool)await Evaluator.EvaluateCode((string)attr.Value, BusinessObj);

                        switch (attr.Key)
                        {
                            case "sdk-show":
                                if (field.Hidden != !result)
                                {
                                    field.Hidden = !result;
                                    shouldUpdate = true;
                                }
                                break;
                            case "sdk-hide":
                                if (field.Hidden != result)
                                {
                                    field.Hidden = result;
                                    shouldUpdate = true;
                                }
                                break;
                            case "sdk-required":
                                if (field.Required != result)
                                {
                                    field.Required = result;
                                    shouldUpdate = true;
                                }
                                break;
                            case "sdk-readonly":
                            case "sdk-disabled":
                                if (field.Disabled != result)
                                {
                                    field.Disabled = result;
                                    shouldUpdate = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }

                if (shouldUpdate)
                {
                    _ = InvokeAsync(() => StateHasChanged());
                }
            });
        }

        /// <summary>
        /// This method initializes the view.
        /// </summary>
        /// <param name="bName">The business name. If null, the method uses the default value from BusinessName.</param>

        protected async Task InitView(string bName = null)
        {
            Loading = true;
            if (bName == null)
            {
                bName = BusinessName;
            }
            await CheckPermissions().ConfigureAwait(true);
            await CreateRelationshipAttachment().ConfigureAwait(true);

            string metadata = CreateMetadata(bName);
                
            if (String.IsNullOrEmpty(metadata))
            {
                //ErrorMsg = "No hay definición para la vista de detalle";
                ErrorMsg = "Custom.Generic.ViewdefNotFound";
                ErrorList.Add(ErrorMsg);
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
                if(BusinessObj.GetType().GetProperty("DynamicEntities") != null && BusinessObj.DynamicEntities != null && BusinessObj.DynamicEntities.Count > 0){
                    AddPanels(PanelsCollapsable);
                }

                setViewContext(Panels);
                if (FormViewModel.Relationships != null && FormViewModel.Relationships.Count > 0)
                {
                    foreach (var relationship in FormViewModel.Relationships)
                    {
                        if (String.IsNullOrEmpty(relationship.ResourceTag))
                        {
                            relationship.ResourceTag = $"{BusinessName}.Relationship.{relationship.Name}";
                        }
                        var canListRel = await FeaturePermissionService.CheckUserActionPermission(relationship.RelatedBusiness, enumSDKActions.Detail, AuthenticationService).ConfigureAwait(true);
                        relationship.Enabled = canListRel;
                    }
                }
                ModelLoaded = true;
            }
            await EvaluateButtonAttributes().ConfigureAwait(true);
            EvaluateDynamicAttributes();
            Loading = false;
            StateHasChanged();
        }

        private string CreateMetadata(string bName)
        {
            string result = "";
            if(String.IsNullOrEmpty(ViewdefName)){
                _viewdefName = "detail";
            }else{
                _viewdefName = ViewdefName;
            }
            var metadata = BackendRouterService.GetViewdef(bName, _viewdefName);
            if (IsSubpanel && String.IsNullOrEmpty(metadata)){
                metadata = BackendRouterService.GetViewdef(bName, "related_default");
                result = metadata;
            }
            if (String.IsNullOrEmpty(metadata) && String.Equals(_viewdefName,"related_detail", StringComparison.Ordinal))
            {
                metadata = BackendRouterService.GetViewdef(bName, "detail");
                result = metadata;
            }
            if(String.IsNullOrEmpty(metadata))
            {
                metadata = BackendRouterService.GetViewdef(bName, "default");
                result = metadata;
            }
            return result;
        }

        private void AddPanels(List<Panel> panels){
            
            foreach(var item in BusinessObj.DynamicEntities){
                var index = BusinessObj.DynamicEntities.IndexOf(item);
                var panel = new Panel();
                panel.ResourceTag = item.Name;
                var fields = new List<FieldOptions>();
                int rowidGroup = item.Rowid;
                panel.RowidGroupDynamicEntity = rowidGroup;
                foreach(var property in item.DynamicObject.GetType().GetProperties()){
                    var field = new FieldOptions();
                    var name = $"DynamicEntities[{index}].DynamicObject.{property.Name}";
                    field.Name = name;
                    field.ResourceTag = property.Name;
                    Dictionary<string, int> colSize = new Dictionary<string, int>();
                    colSize.Add("MD", 4);
                    colSize.Add("SM", 4);
                    colSize.Add("XS", 4);
                    field.ColSize = colSize;
                    field.ViewContext = "DetailView";
                    fields.Add(field);
                }                
                panel.Fields = fields;
                panels.Add(panel);
            }
        }
        public async Task<bool> DeleteGroup(int rowid)
        {
            var response = await BusinessObj.Backend.Call("DeleteGroupDynamicEntity", rowid);
            if(response.Success){
                return true;
            }
            else{
                return false;
            }
        }

        private FieldTypes GetTypesField(dynamic dataType)
        {
            switch(dataType){
                case 0:
                    return FieldTypes.CharField;
                case 1:
                    return FieldTypes.DecimalField;
                case 2:
                    return FieldTypes.DateTimeField;
                default:
                    return FieldTypes.CharField;
            }
        }

        private async Task EvaluateButtonAttributes()
        {
            if(FormViewModel.Buttons != null){
                ExtraButtons = new List<Button>();
                foreach (var button in FormViewModel.Buttons){
                    if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-disabled")){
                        var disabled = await evaluateCodeButtons(button, "sdk-disabled");
                        button.Disabled = disabled;
                    }
                    if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-hide")){
                        var hidden = await evaluateCodeButtons(button, "sdk-hide");
                        button.Hidden = hidden;
                    }
                    if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-show")){
                        var show = await evaluateCodeButtons(button, "sdk-show");
                        button.Hidden = !show;
                    }
                    if(button.Id != null){
                        if(Enum.TryParse<enumTypeButton>(button.Id, out enumTypeButton typeButton)){
                            switch (typeButton)
                            {
                                case enumTypeButton.Create:
                                    CreateButton = button;
                                    break;
                                case enumTypeButton.Duplicate:
                                    DuplicateButton = button;
                                    break;
                                case enumTypeButton.Edit:
                                    EditButton = button;
                                    break;
                                case enumTypeButton.List:
                                    ListButton = button;
                                    break;
                                case enumTypeButton.Delete:
                                    DeleteButton = button;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }else{
                        ExtraButtons.Add(button);
                    }

                    HasExtraButtons = ExtraButtons.Any(x => string.IsNullOrEmpty(x.Id));
                }
                //_ = InvokeAsync(() => StateHasChanged());
            }
        }

        public async Task<bool> evaluateCodeButtons(Button button, string condition){
            bool disabled = button.Disabled;
            var sdkDisable = button.CustomAttributes[condition];
            if(sdkDisable != null){
                var eject = (bool)await Evaluator.EvaluateCode((string)sdkDisable, BusinessObj); //revisar
                if(eject != null){
                    disabled = eject;
                }
            }
            return disabled;
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            bool changeBusinessName = parameters.DidParameterChange(nameof(BusinessName), BusinessName);

            await base.SetParametersAsync(parameters);

            if(BusinessName != null && (changeBusinessName)){
                Loading = false;
                this.ModelLoaded = false;
                ErrorMsg = "";
                ErrorList = new List<string>();
                await InitView(BusinessName);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //InitView();
        }

        private async Task CreateRelationshipAttachment()
        {
            var attachment = BusinessObj.BaseObj.GetType().GetProperty("RowidAttachment");
            if(attachment == null || BusinessName == "BLAttachmentDetail"){
                return;
            }
            ContainAttachments = true;
        }
        private void GoToCreate()
        {
            NavManager.NavigateTo($"{BusinessName}/create/");
        }

        private void GoToDuplicate()
        {
            NavManager.NavigateTo($"{BusinessName}/create/{BusinessObj.BaseObj.Rowid}/");
        }

        private void GoToEdit()
        {
            NavManager.NavigateTo($"{BusinessName}/edit/{BusinessObj.BaseObj.Rowid}/");
        }

        private void GoToList()
        {
            NavManager.NavigateTo($"{BusinessName}/");
        }

        private async Task DeleteBusiness()
        {
            dynamic result = null;
            try{
                result = await BusinessObj.DeleteAsync();
            }catch(Exception ex){
                ErrorMsg = ex.Message;
                ErrorList.Add(ErrorMsg);
            }

            if (result != null && result.Errors.Count > 0)
            {
                foreach(var error in result.Errors){
                    NotificationService.ShowError(error.Message);
                    ErrorList.Add(error.Message);
                }
                // ErrorMsg = "<ul>";
                // foreach (var error in result.Errors)
                // {
                //     ErrorMsg += $"<li>";
                //     ErrorMsg += !string.IsNullOrWhiteSpace(error.Attribute) ? $"{error.Attribute} - " : string.Empty;
                //     ErrorMsg += error.Message.Replace("\n", "<br />");
                //     ErrorMsg += $"</li>";
                // }
                // ErrorMsg += "</ul>";


                return;
            }
            if (result != null)
            {
                if (IsSubpanel)
                {
                    dialogService.Close(false);
                }
                else
                {
                    string uri = NavManager.Uri;
                    NavigationService.RemoveItem(uri);
                    NavManager.NavigateTo($"{BusinessName}/");
                }
            }
        }


        private void OnClickCustomButton(Button button)
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
                Evaluator.EvaluateCode(button.Action, BusinessObj);
            }
        }

        protected virtual async Task CheckPermissions()
        {
            if (FeaturePermissionService != null && !string.IsNullOrEmpty(BusinessName))
            {
                if(IsSubpanel && BusinessName.Equals("BLAttachmentDetail"))
                {
                    try
                    {
                        CanAcess = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.AccessAttachment, AuthenticationService);
                        CanCreate = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.UploadAttachment, AuthenticationService);
                        CanDelete = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.DeleteAttachment, AuthenticationService);
                        CanDetail = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.DownloadAttachment, AuthenticationService);
                    }
                    catch (System.Exception)
                    {
                    }
                }else
                {
                    try
                    {
                        CanAcess = await FeaturePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.Detail, AuthenticationService);
                        CanCreate = await FeaturePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.Create, AuthenticationService);
                        CanEdit = await FeaturePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.Edit, AuthenticationService);
                        CanDelete = await FeaturePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.Delete, AuthenticationService);
                        CanDetail = await FeaturePermissionService.CheckUserActionPermission(BusinessName, enumSDKActions.Detail, AuthenticationService);
                    }
                    catch (System.Exception)
                    {
                    }
                }

                if (!CanDetail)
                {
                    ErrorMsg = "Custom.Generic.Unauthorized";
                    NotificationService.ShowError("Custom.Generic.Unauthorized");
                    ErrorList.Add("Custom.Generic.Unauthorized");
                    if(!IsSubpanel){
                        // NavigationService.NavigateTo("/", replace: true);
                    }
                }
            }
        }
    }
}

