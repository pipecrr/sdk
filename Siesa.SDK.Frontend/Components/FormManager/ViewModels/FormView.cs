using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Microsoft.JSInterop;
using Siesa.SDK.Business;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DevExpress.DataAccess.Native.Web;
using Siesa.SDK.Frontend.Utils;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Shared.DTOS;
using Siesa.SDK.Frontend.Components.FormManager.Fields;
using Siesa.SDK.Frontend.Extension;
using Microsoft.Extensions.Configuration;
using Siesa.Global.Enums;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Shared.Utilities;

namespace Siesa.SDK.Frontend.Components.FormManager.ViewModels
{
    public abstract class FormView : ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public string BusinessNameParent { get; set; }
        [Parameter]
        public FormView ParentForm { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }
        [Parameter] 
        public bool IsSubpanel { get; set; }
        [Parameter]
        public Type BusinessObjAType { get; set; }
        [Inject] public  Radzen.DialogService dialogService { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }
        [Inject] public NavigationService NavigationService { get; set; }
        [Inject] public SDKNotificationService NotificationService { get; set; }
        [Inject] public IConfiguration configuration { get; set; }
        private bool UseRoslynToEval { get; set; }

        [Inject] protected IAuthenticationService AuthenticationService { get; set; }

        [Inject] public SDKGlobalLoaderService GlobalLoaderService { get; set; }

        protected FormViewModel FormViewModel { get; set; } = new FormViewModel();
        /// <summary>
        /// Gets or sets the config detail view model.
        /// </summary>
        protected ListViewModel DetailConfig { get; set; } = new ListViewModel();

        public List<Panel> Panels {get { return FormViewModel.Panels; } }
        public List<Panel> PanelsCollapsable = new List<Panel>();
        public bool ShowAditionalFields { get; set; }

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
        [Parameter]
        public bool IsTableA { get; set; }
        [Parameter]
        public long RowidCompany { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the business object is a document.
        /// </summary>
        public bool IsDocument { get; set; }
        
        public int CountUnicErrors = 0;

        private string _viewdefName = "";

        public bool ContainAttachments = false;

        protected bool loadDefaultViewdef = true;

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
        protected bool CanAcess;
        public bool FormHasErrors = false;

        public Dictionary<string, FileField> FileFields = new Dictionary<string, FileField>();
        public SDKFileUploadDTO DataAttatchmentDetail { get; set; }

        public bool ClickInSave { get; set; }
        public bool HasExtraButtons { get; set; }
        public List<Button> ExtraButtons { get; set; }
        public Button SaveButton { get; set; }
        public bool isOnePanel { get; set; }
        internal bool HasTableA;
        internal Type _businessObjAType;
        internal string BusinessNameA { get; set; }
        public dynamic BaseObjA { get; set; }
        public dynamic BusinessObjA { get; set; }
        public List<FormView> FormViewsTablesA { get; set; } = new List<FormView>();
        internal List<E00201_Company> Companies { get; set; } = new List<E00201_Company>();
        /// <summary>
        /// Gets or sets the delete button config. 
        /// </summary>
        protected Button ButtonDelete {get; set;  }
        /// <summary>
        /// Gets or sets the create button config. 
        /// </summary>
        protected Button ButtonCreate {get; set;  }
        /// <summary>
        /// Gets or sets the reference grid.
        /// </summary>
        public dynamic RefGrid { get; set; }
        protected virtual async Task CheckPermissions()
        {
            if (FeaturePermissionService != null && !String.IsNullOrEmpty(BusinessName))
            {
                try
                {
                    string businessName = BusinessName;
                    if (IsTableA)
                    {
                        businessName = BusinessNameParent;
                    }
                    CanAcess = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Detail, AuthenticationService).ConfigureAwait(true);
                    CanCreate = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Create, AuthenticationService).ConfigureAwait(true);
                    CanEdit = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Edit, AuthenticationService).ConfigureAwait(true);
                    CanDelete = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Delete, AuthenticationService).ConfigureAwait(true);
                    CanDetail = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Detail, AuthenticationService).ConfigureAwait(true);
                }
                catch (System.Exception)
                {
                }
            }
        }
        private void CreateRelationshipAttachment()
        {
            try
            {
                var attachment = BusinessObj.BaseObj.GetType().GetProperty("RowidAttachment");
                if(attachment == null || BusinessName == "BLAttachmentDetail"){
                    return;
                }
                ContainAttachments = true;
            }
            catch (System.Exception)
            {
                ContainAttachments = false;
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
            if (IsSubpanel && String.IsNullOrEmpty(data)){
                data = BackendRouterService.GetViewdef(businessName, "related_default");
            }
            if (String.IsNullOrEmpty(data) && _viewdefName != DefaultViewdefName)
            {
                data = BackendRouterService.GetViewdef(businessName, DefaultViewdefName);
            }
            if(String.IsNullOrEmpty(data) && loadDefaultViewdef){
                _viewdefName = "default";
                data = BackendRouterService.GetViewdef(businessName, _viewdefName);
            }
            return data;
        }

        private void SetViewContext(List<Panel> panels, DynamicViewType viewType)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if(string.IsNullOrEmpty(panels[i].ResourceTag))
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
                    SetViewContextField(panels[i].Fields[j], viewType);
                }
                if (panels[i].SubViewdef != null && panels[i].SubViewdef.Panels.Count > 0)
                {
                    SetViewContext(panels[i].SubViewdef.Panels, viewType);
                }                
            }
        }

        private void SetViewContextField(FieldOptions field, DynamicViewType viewType){
            if(viewType == DynamicViewType.Detail && String.IsNullOrEmpty(field.ViewContext)){
                field.ViewContext = "DetailView";
            }
            field.GetFieldObj(BusinessObj);

            if (field.Fields == null || field.Fields.Count <= 0) return;

            foreach (var item in field.Fields.Select((value, i) => (value, i)))
            {
                SetViewContextField(item.value, viewType);
            }
        }

        public void Refresh(bool Reload = false){
            EvaluateDynamicAttributes(null);
            EvaluateButtonAttributes();
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
            CreateRelationshipAttachment();
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
            var metadata = GetViewdef(bName);
            if (String.IsNullOrEmpty(metadata))
            {
                ErrorMsg = $"Custom.Generic.ViewdefNotFound";
                ErrorList.Add($"Custom.Generic.ViewdefNotFound");
            }
            else
            {
                CreateFormViewModel(metadata);
            }
            try
            {
                SetViewContext(FormViewModel.Panels, ViewContext);
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
            if(IsDocument)
            {
                DetailConfig = FormViewModel.DetailConfig;
                AddOnChangeCell();
                ButtonDelete = DetailConfig.Buttons.FirstOrDefault(x => x.Id.Equals("Delete",StringComparison.Ordinal));
                ButtonCreate = DetailConfig.Buttons.FirstOrDefault(x => x.Id.Equals("Create",StringComparison.Ordinal));
                BusinessObj.ExtraDetailFields = DetailConfig.Fields.Select(x => x.Name).ToList();
                
                await BusinessObj.InitializeChilds().ConfigureAwait(true);
            }
            Loading = false;
            if(EditFormContext == null)
            {
                EditFormContext = new EditContext(BusinessObj);
            }
            EditFormContext.OnFieldChanged += EditContext_OnFieldChanged;
            _messageStore = new ValidationMessageStore(EditFormContext);
            EditFormContext.OnValidationRequested += EditFormContext_OnValidationRequested;
            EvaluateDynamicAttributes(null);
            await EvaluateButtonAttributes().ConfigureAwait(true);
            BusinessObj.ParentComponent = this;
            StateHasChanged();
        }
        private void CreateFormViewModel(string metadata)
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
            if(BusinessObj.GetType().GetProperty("DynamicEntities") != null && BusinessObj.DynamicEntities != null ){
                ShowAditionalFields = true;
                if(BusinessObj.DynamicEntities.Count > 0){
                    AddPanels(PanelsCollapsable);
                }
            }
        }

        private void EditFormContext_OnValidationRequested(object sender, ValidationRequestedEventArgs e)
        {
            _messageStore.Clear();
            if(FormViewsTablesA != null && FormViewsTablesA.Any()){
                FormViewsTablesA.Select(formView => formView.EditFormContext).ToList().ForEach(editContext => {
                    _messageStore.Clear();
                    editContext.Validate();
                    if(editContext.GetValidationMessages().Any())
                    {
                        foreach (var item in editContext.GetValidationMessages().Distinct())
                        {
                            _messageStore.Add(editContext.Field(item), item);
                        }
                    }
                });
            }
        }
        internal async Task InitViewTableA()
        {
            dynamic businessObj = null;
            EditContext editFormContext = null;
            if (ParentForm.FormViewsTablesA.Any())
            {
                foreach (var formview in ParentForm.FormViewsTablesA)
                {
                    if(formview.RowidCompany == RowidCompany)
                    {
                        businessObj = formview.BusinessObj;
                        editFormContext = formview.EditFormContext;
                        break;
                    }
                }
            }
            if (BusinessObjAType != null && businessObj == null)
            {
                BusinessObj = Activator.CreateInstance(BusinessObjAType, AuthenticationService);
                if (BusinessObj != null)
                {
                    Int16? rowidCompany = (Int16?)(RowidCompany);
                    dynamic baseObj = Activator.CreateInstance(BusinessObj.BaseObj.GetType());
                    if (ParentForm.BusinessObj.BaseObj.Rowid > 0)
                    {
                        string where = $"RowidCompany == {RowidCompany} && RowidRecord == {ParentForm.BusinessObj.BaseObj.Rowid}";
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
                
                ParentForm.FormViewsTablesA.Add(this);
            }else
            {
                BusinessObj = businessObj;
            }
            if (editFormContext != null){
                EditFormContext = editFormContext;
            }
        }

        private async Task VerifyTableA()
        {
            BusinessNameA = BusinessObj.GetType().Name.Replace("BL", "BLA");
            _businessObjAType = Utilities.SearchType(BusinessObj.GetType().Namespace + "." + BusinessNameA);
            if (_businessObjAType != null)
            {
                HasTableA = true;
                var bL = BackendRouterService.GetSDKBusinessModel("BLSDKCompany",AuthenticationService);
                int rowidCompanyGroup = AuthenticationService.GetRowidCompanyGroup();
                var dataCompany = await bL.GetData(null, null, $"RowidCompanyGroup == {rowidCompanyGroup}").ConfigureAwait(true);
                Companies = dataCompany.Data.Select(x => JsonConvert.DeserializeObject<E00201_Company>(x)).ToList();
            }
        }

        private void AddOnChangeCell()
        {
            foreach (var item in DetailConfig.Fields)
            {
                if(item.CustomAttributes == null){
                    item.CustomAttributes = new Dictionary<string, object>();
                }
                if (!item.CustomAttributes.TryGetValue("sdk-change-cell", out object value))
                {
                    item.CustomAttributes.Add("sdk-change-cell", "SdkOnChangeCell");
                }
            }
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
        private async Task EvaluateButtonAttributes()
        {
            if(FormViewModel.Buttons != null ){
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
                                case enumTypeButton.Save:
                                    SaveButton = button;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }else{
                        ExtraButtons.Add(button);
                    }
                }

                HasExtraButtons = ExtraButtons.Any(x => string.IsNullOrEmpty(x.Id));
                _ = InvokeAsync(() => StateHasChanged());
            }
        }
        
        /// <summary>
        /// Evaluates a specified condition (sdk-disabled, sdk-hide, sdk-show) for a <paramref name="button"/> and returns the result.
        /// </summary>
        /// <param name="button">The <see cref="Button"/> object to evaluate the condition for.</param>
        /// <param name="condition">The name of the condition stored in the custom attributes of the <paramref name="button"/>.</param>
        /// <param name="data">An optional dynamic object that may be passed to the evaluation process.</param>
        /// <returns>
        /// Returns true if the condition evaluates to true; otherwise, returns false.
        /// </returns>
        public async Task<bool> EvaluateCodeButtons(Button button, string condition, dynamic data = null)
        {
            bool result = false;
            var sdkAttr = button?.CustomAttributes[condition];
            if(sdkAttr != null){
                string attrValue = sdkAttr.ToString();
                if (data != null)
                {
                    attrValue = AttrCode(attrValue, data);
                    if(!string.IsNullOrEmpty(attrValue)){
                        result = await EjectMethod(data, attrValue, true).ConfigureAwait(true);
                    }
                }
                else
                {
                    var eject = (bool)await Evaluator.EvaluateCode(attrValue, BusinessObj);
                    result = eject;
                }
            }
            return result;
        }

        private string AttrCode(string attrValue, dynamic data)
        {
            string result = attrValue;            
            if (result != null && result.Contains("data_detail", StringComparison.Ordinal))
            {
                var indexData = BusinessObj.ChildObjs.IndexOf(data);
                if(indexData == -1){
                    return null;
                }
                result = result.Replace("data_detail", $"ChildObjs[{indexData}]",
                    StringComparison.Ordinal);
            }

            return result;
        }

        private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            _messageStore.Clear(e.FieldIdentifier);
            EvaluateDynamicAttributes(e);
            EvaluateButtonAttributes();
        }

        private void EvaluateDynamicAttributes(FieldChangedEventArgs e)
        {
            foreach (var item in Panels.Select((value, i) => (value, i)))
            {
                var panel = item.value;
                if (panel.Fields == null)
                {
                    continue;
                }
                EvaluateFields(panel.Fields);
            }
        }

        private void EvaluateFields(List<FieldOptions> panelFields)
        {
            foreach (var field in panelFields)
            {
                _ = Task.Run(async () =>
                {
                    bool shouldUpdate = await UpdateCustomAttr(field).ConfigureAwait(true);
                    if(shouldUpdate)
                    {
                        _ = InvokeAsync(() => StateHasChanged());
                    }
                });
            }
        }
        /// <summary>
        /// Updates the custom attributes of a <paramref name="field"/> based on evaluation results.
        /// </summary>
        /// <param name="field">The <see cref="FieldOptions"/> object representing the field with custom attributes.</param>
        /// <param name="isFieldDocument">Indicates whether the field is a document field.</param>
        /// <param name="data">Additional data used for evaluation, applicable when <paramref name="isFieldDocument"/> is true.</param>
        /// <returns>
        /// Returns true if any custom attribute of the field has been updated, otherwise returns false.
        /// </returns>
        public async Task<bool> UpdateCustomAttr(FieldOptions field, bool isFieldDocument = false, dynamic data = null)
        {
            if (field == null)
            {
                return false;
            }
            
            if(field.Fields != null && field.Fields.Count > 0)
            {
                EvaluateFields(field.Fields);
            }

            if(field.CustomAttributes == null){
                return false;
            }
            
            bool shouldUpdate = false;
            
            var fieldCustomAttr = field.CustomAttributes.Where(x => x.Key.StartsWith("sdk-",StringComparison.Ordinal) && x.Key != "sdk-change");

            foreach (var attr in fieldCustomAttr)
            {
                try
                {
                    shouldUpdate |= await UpdateFieldBasedOnAttr(field, attr, isFieldDocument, data);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            return shouldUpdate;
        }
        private async Task<bool> UpdateFieldBasedOnAttr(FieldOptions field, KeyValuePair<string, object> attr, bool isFieldDocument, dynamic data)
        {
            string attrValue = (string)attr.Value;

            if (isFieldDocument && data != null && attrValue.Contains("data_detail", StringComparison.Ordinal))            
            {
                var indexData = BusinessObj.ChildObjs.IndexOf(data);
                attrValue = attrValue.Replace("data_detail", $"ChildObjs[{indexData}]", StringComparison.Ordinal);
            }

            var result = (bool)await Evaluator.EvaluateCode(attrValue, BusinessObj);

            switch (attr.Key)
            {
                case "sdk-show":
                    if (field.Hidden != !result)
                    {
                        field.Hidden = !result;
                        return true;
                    }
                    break;
                case "sdk-hide":
                    if (field.Hidden != result)
                    {
                        field.Hidden = result;
                        return true;
                    }
                    break;
                case "sdk-required":
                    if (field.Required != result)
                    {
                        field.Required = result;
                        return true;
                    }
                    break;
                case "sdk-readonly":
                case "sdk-disabled":
                    if (field.Disabled != result)
                    {
                        field.Disabled = result;
                        return true;
                    }
                    break;
                default:
                    break;
            }

            return false;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            try
            {
                UseRoslynToEval = configuration.GetValue<bool>("UseRoslynToEval");
            }
            catch (System.Exception ex)
            {
            }
            //await InitView();
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            bool changeViewContext = parameters.DidParameterChange(nameof(ViewContext), ViewContext);
            bool changeBusinessName = parameters.DidParameterChange(nameof(BusinessName), BusinessName);

            await base.SetParametersAsync(parameters).ConfigureAwait(true);

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
            try{
                await fileField.Upload();
            }catch(Exception ex){
                //TODO: pdte por revision 
                SavingFile = false;
                return 0;
            }
            var horaInicio = DateTime.Now.Minute;
            while(SavingFile){
                await Task.Delay(100);
                if (DateTime.Now.Minute - horaInicio >= 2){
                    throw new Exception("Timeout");
                }
            }
            if(DataAttatchmentDetail != null){
                try{
                    var result = await BusinessObj.SaveAttachmentDetail(DataAttatchmentDetail, rowid);
                    return result;
                }catch(Exception ex){
                    SavingFile = false;
                    return rowid;
                }
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
            if(CountUnicErrors>0){
                GlobalLoaderService.Hide();
                Saving = false;
                var existeUniqueIndexValidation = NotificationService.Messages.Where(x => x.Summary == "Custom.Generic.UniqueIndexValidation").Any();
                if(!existeUniqueIndexValidation){
                    NotificationService.ShowError("Custom.Generic.UniqueIndexValidation");
                    ErrorList.Add("Custom.Generic.UniqueIndexValidation");
                }
                return;
            }
            GlobalLoaderService.Show();
            if(FileFields.Count>0)
            {
                await SavingFiles().ConfigureAwait(true);
            }
            dynamic result = null;
            try{
                result = await BusinessObj.ValidateAndSaveAsync();
                if (FormViewsTablesA.Any())
                {
                    foreach (var formView in FormViewsTablesA)
                    {
                        dynamic resultA = null;
                        try
                        {
                            dynamic baseObj = formView.BusinessObj.BaseObj;
                            Type typeRowidRecord = baseObj.GetType().GetProperty("RowidRecord").PropertyType; 
                            dynamic rowidRecord = Convert.ChangeType(result.Rowid, typeRowidRecord);
                            baseObj.RowidRecord = rowidRecord;
                            formView.BusinessObj.BaseObj = baseObj;
                            resultA = await formView.BusinessObj.ValidateAndSaveAsync();
                            result.Errors.AddRange(resultA.Errors);
                        }
                        catch(System.Exception ex)
                        {
                            GlobalLoaderService.Hide();
                            Saving = false;
                            ErrorMsg = ex.Message;
                            ErrorList.Add("Exception: " + ex.Message);
                            return;
                        }
                    }
                }
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
                
                foreach (var error in result.Errors)
                {
                    FieldIdentifier fieldIdentifier;
                    Type editFormCurrentType = EditFormContext.Model.GetType();
                    bool fieldInContext = false;
                    //check if attribute is in Model
                    if(editFormCurrentType.GetProperty(error.Attribute) != null)
                    {
                        fieldIdentifier = new FieldIdentifier(EditFormContext.Model, error.Attribute);
                        fieldInContext = true;
                    }
                    else if(((string)error.Attribute).Split('.').Count() > 1)
                    {
                        var attr = ((string)error.Attribute).Split('.');
                        var fieldExists = true;
                        foreach(string item in attr)
                        {
                            if(editFormCurrentType.GetProperty(item) != null)
                            {
                                editFormCurrentType = editFormCurrentType.GetProperty(item).PropertyType;
                            }else{
                                fieldExists = false;
                                break;
                            }

                        }
                        if(fieldExists)
                        {
                            fieldIdentifier = new FieldIdentifier(EditFormContext.Model, error.Attribute);
                            fieldInContext = true;
                        }
                    }
                    if(fieldInContext)
                    {
                        _messageStore.Add(fieldIdentifier, (string)error.Message);
                    }else{
                        ErrorList.Add("Exception: "+error.Message);
                    }
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
        
        /// <summary>
        /// Saves the attached files to the business object.
        /// </summary>
        /// <remarks>
        /// This function iterates through the collection of attached files represented by the "FileFields" dictionary.
        /// For each attached file, it checks whether it already exists in the business object or is new. 
        /// If it is new, it is saved, and its "Rowid" identifier is updated.
        /// </remarks>        
        public async Task SavingFiles()
        {
            foreach (var item in FileFields)
            {
                var fileField = item.Value;
                var rowidAttatchment = 0;
                var field = item.Key;
                dynamic property = BusinessObj.BaseObj.GetType().GetProperty(field).GetValue(BusinessObj.BaseObj);
                if(property != null){ 
                    var rowidProp = property.GetType().GetProperty("Rowid").GetValue(property);
                    if(rowidProp != null)
                        rowidAttatchment = await savingAttachment(fileField, (int)rowidProp).ConfigureAwait(true);
                }else{
                    rowidAttatchment = await savingAttachment(fileField).ConfigureAwait(true);
                }
                if(rowidAttatchment > 0){
                    var attatchmentDetail = Activator.CreateInstance(BusinessObj.BaseObj.GetType().GetProperty(field).PropertyType);
                    attatchmentDetail.GetType().GetProperty("Rowid").SetValue(attatchmentDetail, rowidAttatchment);
                    BusinessObj.BaseObj.GetType().GetProperty(field).SetValue(BusinessObj.BaseObj, attatchmentDetail);
                }
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
            ClickInSave = true;
        }
        protected void HandleInvalidSubmit()
        {
            //ErrorMsg = @"Form data is invalid";
            FormHasErrors = true;
            NotificationService.ShowError("Custom.Generic.FormError");
            var existeUniqueIndexValidation = NotificationService.Messages.Where(x => x.Summary == "Custom.Generic.UniqueIndexValidation").Any();
            if(existeUniqueIndexValidation){
                ErrorList.Add("Custom.Generic.UniqueIndexValidation");
            }else{
                ErrorList.Clear();
            }
            ClickInSave = true;
        }
        protected void GoToList()
        {
            NavManager.NavigateTo($"{BusinessName}/");
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
                _ = InvokeAsync(() => StateHasChanged());
            }
        }
        
        protected string GetSaveBtnIcon()
        {
            return Saving ? "fas fa-spinner fa-pulse" : "fa-solid fa-floppy-disk";
        }
        /// <summary>
        /// Method to add a new row to the grid.
        /// </summary>
        public async Task ClickAddRow()
        {
            Type typeChild = BusinessObj.ChildType();
            dynamic obj = Activator.CreateInstance(typeChild);
            dynamic ListChildObj = typeof(List<>).MakeGenericType(new Type[] { typeChild });
            if(BusinessObj.ChildObjs == null)
            {
                BusinessObj.ChildObjs = Activator.CreateInstance(ListChildObj);
            }

            if (!String.IsNullOrEmpty(DetailConfig.ActionAddRow))
            {
                obj = await EjectMethod(obj, DetailConfig.ActionAddRow, true).ConfigureAwait(true);
            }
            BusinessObj.ChildObjs.Add(obj);
            StateHasChanged();
            RefGrid.Reload();
        }

        /// <summary>
        /// Method to delete a row from the grid.
        /// </summary>
        /// <param name="obj">The object to delete.</param>
        public async Task ClickDeleteRow(dynamic obj){
            bool delete = true;
            if (!String.IsNullOrEmpty(DetailConfig.ActionDeleteRow))
            {
                delete = await EjectMethod(obj, DetailConfig.ActionDeleteRow, true).ConfigureAwait(true);
            }
            if(delete){
                BusinessObj.ChildObjs.Remove(obj);
                if(obj.Rowid != null && obj.Rowid > 0){
                    BusinessObj.ChildObjsDeleted.Add(obj);
                    BusinessObj.ChildRowidsUpdated.Remove(obj.Rowid);
                }
            }
            StateHasChanged();
            RefGrid.Reload();
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
    }
}
