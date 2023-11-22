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
using System.Runtime.CompilerServices;
using Siesa.SDK.Shared.Utilities;

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
        public Type BusinessObjAType { get; set; }

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
        [Parameter]
        public string BusinessNameParent { get; set; }
        
        [Parameter]
        public DetailView ParentDetail { get; set; }
        
        [Parameter]
        public List<string> ParentBaseObj { get; set; }
        [Parameter]
        public bool IsTableA { get; set; }
        [Parameter]
        public long RowidCompany { get; set; }

        [Parameter]
        public bool HideRelationshipContainer { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the business object is a document.
        /// </summary>
        public bool IsDocument { get; set; }

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
        /// <summary>
        /// Gets or sets the config detail view model.
        /// </summary>
        protected ListViewModel DetailConfig { get; set; } = new ListViewModel();
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
        /// <summary>
        /// Gets or sets the reference grid.
        /// </summary>
        public dynamic RefGrid { get; set; }

        private bool HasExtraButtons { get; set; }
        private List<Button> ExtraButtons { get; set; }
        private Button CreateButton { get; set; }
        private Button DuplicateButton { get; set; }
        private Button EditButton { get; set; }
        private Button ListButton { get; set; }
        private Button DeleteButton { get; set; }
        private string _viewdefName;
        
        internal bool HasTableA;
        internal Type InternalBusinessObjAType;
        internal string BusinessNameA { get; set; }
        public List<DetailView> DetailViewsTablesA { get; set; } = new List<DetailView>();
        internal List<E00201_Company> Companies { get; set; } = new List<E00201_Company>();
        private List<string> StackTrace = new ();

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
                if (string.IsNullOrEmpty(panels[i].ResourceTag))
                {
                    if(i == 0 && string.IsNullOrEmpty(panels[i].Name))
                    {
                        panels[i].ResourceTag = $"Custom.Viewdef.{_viewdefName}.panel";
                    }else{
                        panels[i].ResourceTag = $"{BusinessName}.Viewdef.{_viewdefName}.Panel.{panels[i].Name}";
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
                        StackTrace.Add(ex.Message);
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
            IsDocument = Utilities.CheckIsDocument(BusinessObj, typeof(BLFrontendDocument<,,>));
            if (bName == null)
            {
                bName = BusinessName;
            }
            
            if (!IsTableA)
            {
                await VerifyTableA().ConfigureAwait(true);
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
            if(IsDocument)
            {
                List<string> extraDetailFields = new ();
                DetailConfig = FormViewModel.DetailConfig;
                DetailConfig.Fields.ForEach(x =>
                {
                    x.ViewContext = "DetailView";
                    extraDetailFields.Add(x.Name);
                });
                BusinessObj.ExtraDetailFields = extraDetailFields;
                await BusinessObj.InitializeChilds().ConfigureAwait(true);
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
            }
            if (String.IsNullOrEmpty(metadata) && String.Equals(_viewdefName,"related_detail", StringComparison.Ordinal))
            {
                metadata = BackendRouterService.GetViewdef(bName, "detail");
            }
            if(String.IsNullOrEmpty(metadata))
            {
                metadata = BackendRouterService.GetViewdef(bName, "default");
            }
            if(!String.IsNullOrEmpty(metadata)){
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
                        var disabled = await EvaluateCodeButtons(button, "sdk-disabled").ConfigureAwait(true);
                        button.Disabled = disabled;
                    }
                    if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-hide")){
                        var hidden = await EvaluateCodeButtons(button, "sdk-hide").ConfigureAwait(true);
                        button.Hidden = hidden;
                    }
                    if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-show")){
                        var show = await EvaluateCodeButtons(button, "sdk-show").ConfigureAwait(true);
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
        /// <summary>
        /// Evaluates a specified condition (sdk-disabled, sdk-hide, sdk-show) for a <paramref name="button"/> and returns the result.
        /// </summary>
        /// <param name="button">The <see cref="Button"/> object to evaluate the condition for.</param>
        /// <param name="condition">The name of the condition stored in the custom attributes of the <paramref name="button"/>.</param>        
        /// <returns>
        /// Returns true if the condition evaluates to true; otherwise, returns false.
        /// </returns>
        public async Task<bool> EvaluateCodeButtons(Button button, string condition){
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
            if (IsTableA)
            {
                await InitViewTableA().ConfigureAwait(true);
            }
            await base.OnInitializedAsync().ConfigureAwait(true);
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
                //ErrorMsg = ex.Message;
                ErrorList.Add("Custom.Generic.Message.Error");
                StackTrace.Add(ex.Message);
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

        /// <summary>
        /// Handles the click event of a custom button, performing the associated action.
        /// </summary>
        /// <param name="button">The <see cref="Button"/> object representing the clicked button.</param>
        /// <param name="obj">An optional dynamic object that may be passed to the action associated with the button.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task OnClickCustomButton(Button button, dynamic obj = null)
        {
            if (!string.IsNullOrEmpty(button?.Href))
            {
                if (button.Target == "_blank")
                {
                    await JSRuntime.InvokeVoidAsync("window.open", button.Href, "_blank").ConfigureAwait(true);
                }
                else
                {
                    NavManager.NavigateTo(button.Href);
                }


            }
            else if (!string.IsNullOrEmpty(button?.Action))
            {
                await EjectMethod(obj, button.Action).ConfigureAwait(true);
                StateHasChanged();
            }
        }

        protected virtual async Task CheckPermissions()
        {
            if (FeaturePermissionService != null && !string.IsNullOrEmpty(BusinessName))
            {
                if(IsSubpanel && BusinessName.Equals("BLAttachmentDetail", StringComparison.Ordinal))
                {
                    await CheckPermissionsByBussinessName(BusinessName).ConfigureAwait(true);
                }else
                {
                    string businessName = BusinessName;
                    if (IsTableA)
                    {
                        businessName = BusinessNameParent;
                    }
                    CanEdit = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Edit, AuthenticationService).ConfigureAwait(true);
                    await CheckPermissionsByBussinessName(businessName).ConfigureAwait(true);
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

        private async Task CheckPermissionsByBussinessName(string businessName)
        {
            try
            {
                CanAcess = await FeaturePermissionService.CheckUserActionPermission(businessName,
                    enumSDKActions.AccessAttachment, AuthenticationService).ConfigureAwait(true);
                CanCreate = await FeaturePermissionService.CheckUserActionPermission(businessName,
                    enumSDKActions.UploadAttachment, AuthenticationService).ConfigureAwait(true);
                CanDelete = await FeaturePermissionService.CheckUserActionPermission(businessName,
                    enumSDKActions.DeleteAttachment, AuthenticationService).ConfigureAwait(true);
                CanDetail = await FeaturePermissionService.CheckUserActionPermission(businessName,
                    enumSDKActions.DownloadAttachment, AuthenticationService).ConfigureAwait(true);
            }
            catch (System.Exception ex)
            {
                StackTrace.Add(ex.Message);
            }
        }

        /// <summary>
        /// Executes a specified action using an evaluator and method information extracted from the provided object. 
        /// This method can handle asynchronous methods and optionally returns a value based on the 'hasReturn' parameter.
        /// </summary>
        /// <param name="obj">The object on which the action will be executed.</param>
        /// <param name="action">The action to be executed.</param>
        /// <param name="hasReturn">Indicates whether the action has a return value.</param>
        /// <returns>
        /// If 'hasReturn' is true and the action has a return value, the result of the action is returned.
        /// If 'hasReturn' is false or the action is void, no explicit return value is provided.
        /// If the action result is of type bool, it is returned directly.
        /// </returns>
        public async Task<dynamic> EjectMethod(dynamic obj, string action, bool hasReturn = false)
        {
            var eject = await Evaluator.EvaluateCode(action, BusinessObj);
            MethodInfo methodInfo = (MethodInfo)(eject?.GetType().GetProperty("Method")?.GetValue(eject));
            if(methodInfo != null){
                if(methodInfo.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false).Length > 0){
                    if (hasReturn)
                    {
                        return await eject(obj).ConfigureAwait(true);
                    }else{
                        await eject(obj).ConfigureAwait(true);
                    }
                }else{
                    if (hasReturn)
                    {
                        return eject(obj);
                    }else{
                        eject(obj);
                    }
                }
            }else if (eject != null && eject.GetType() == typeof(bool))
            {
                return eject;
            }
            return obj;
        }
        
        private async Task VerifyTableA()
        {
            BusinessNameA = BusinessObj.GetType().Name.Replace("BL", "BLA");
            InternalBusinessObjAType = Utilities.SearchType(BusinessObj.GetType().Namespace + "." + BusinessNameA);
            if (InternalBusinessObjAType != null)
            {
                HasTableA = true;
                var bL = BackendRouterService.GetSDKBusinessModel("BLSDKCompany",AuthenticationService);
                int rowidCompanyGroup = AuthenticationService.GetRowidCompanyGroup();
                var dataCompany = await bL.GetData(null, null, $"RowidCompanyGroup == {rowidCompanyGroup}").ConfigureAwait(true);
                Companies = dataCompany.Data.Select(x => JsonConvert.DeserializeObject<E00201_Company>(x)).ToList();
            }
        }
        
        internal async Task InitViewTableA()
        {
            dynamic businessObj = null;
            if (ParentDetail.DetailViewsTablesA.Any())
            {
                foreach (var formview in ParentDetail.DetailViewsTablesA)
                {
                    if(formview.RowidCompany == RowidCompany)
                    {
                        businessObj = formview.BusinessObj;
                        break;
                    }
                }
            }
            if (BusinessObjAType != null && businessObj == null)
            {
                BusinessObj = Activator.CreateInstance(BusinessObjAType, AuthenticationService);
                if (BusinessObj != null)
                {
                    await GenerateBaseObj().ConfigureAwait(true);
                }
                
                ParentDetail.DetailViewsTablesA.Add(this);
            }else
            {
                BusinessObj = businessObj;
            }
        }

        private async Task GenerateBaseObj()
        {
            Int16? rowidCompany = (Int16?)(RowidCompany);
            dynamic baseObj = Activator.CreateInstance(BusinessObj.BaseObj.GetType());
            if (ParentDetail.BusinessObj.BaseObj.Rowid > 0)
            {
                string where = $"RowidCompany == {RowidCompany} && RowidRecord == {ParentDetail.BusinessObj.BaseObj.Rowid}";
                var response = await BusinessObj.GetDataAsync(null, null, where, "");
                var totalCount = response.TotalCount;
                if (totalCount > 0)
                {
                    dynamic data = response.Data[0];
                    baseObj = data;
                }
            }
            else
            {
                baseObj.RowidCompany = rowidCompany;
            }

            BusinessObj.BaseObj = baseObj;
        }
    }
}

