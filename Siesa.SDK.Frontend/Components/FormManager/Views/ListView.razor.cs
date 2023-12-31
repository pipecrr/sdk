using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Microsoft.JSInterop;
using Siesa.SDK.Business;
using System.Threading;
using System.Reflection;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Components.FormManager.Fields;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Utils;
using System.Linq;
using Siesa.SDK.Frontend.Application;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Shared.Utilities;
using Siesa.SDK.Entities;
using Blazored.LocalStorage;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.DependencyInjection;
using Siesa.Global.Enums;
using Newtonsoft.Json;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using Siesa.SDK.Frontend.Components.Flex;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.DTOS;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class ListView : ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }

        [Parameter]
        public bool SetTopBar { get; set; } = true;

        [Parameter]
        public bool IsSubpanel { get; set; }

        [Parameter]
        public List<string> ConstantFilters { get; set; } = new List<string>();

        [Parameter]
        public bool AllowCreate { get; set; } = true;
        [Parameter]
        public bool AllowEdit { get; set; } = true;
        [Parameter]
        public bool AllowDelete { get; set; } = true;
        [Parameter]
        public bool AllowDetail { get; set; } = true;
        [Parameter]
        public bool AllowExport { get; set; } = true;
        [Parameter]
        public bool ShowSearchForm { get; set; } = true;
        [Parameter]
        public bool ShowList { get; set; } = true;
        [Parameter]
        public bool UseFlex { get; set; } = true;
        [Parameter]
        public int FlexTake { get; set; } = 100;
        [Parameter]
        public bool ServerPaginationFlex { get; set; } = true;
        [Parameter]
        public bool ShowLinkTo {get; set;} = false;
        [Parameter]
        public bool FromEntityField {get; set;} = false;
        /// <summary>
        /// Gets or sets of parameters to pass to the Detail View.
        /// </summary>
        [Parameter]
        public bool RedirectDetail { get; set; } = true;
        /// <summary>
        /// Gets or sets of parameters to pass to the Create View.
        /// </summary>
        [Parameter]
        public bool RedirectCreate { get; set; }
        [Parameter]
        public string BLNameParentAttatchment { get; set; }
        [Parameter]
        public bool? ShowActions { get; set; }
        [Inject]
        public ILocalStorageService localStorageService { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get;set; }
        [Inject]
        public UtilsManager UtilsManager { get; set; }
        private FreeForm SearchFormRef;
        private string FilterFlex { get; set; } = "";

        public string SearchFormID = Guid.NewGuid().ToString();

        public string guidListView = "";

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }

        [Inject] public SDKNotificationService NotificationService { get; set; }

        [Inject] public NavigationService NavigationService { get; set; }
        [Inject] public IFeaturePermissionService FeaturePermissionService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public IBackendRouterService BackendRouterService { get; set; }

        [Inject] public SDKDialogService dialogService { get; set; }
        [Inject] public Radzen.DialogService dialogServiceRadzen { get; set; }

        [Inject] public SDKGlobalLoaderService SDKGlobalLoaderService { get; set; }

        public bool Loading;
        public bool LoadingData;
        public bool LoadingSearch;

        private bool _isEditingFlex = false;
        private bool _isSearchOpen = false;
        private bool _showActions = true;
        public String ErrorMsg = "";
        public List<ModelMessagesDTO> ErrorList = new();
        private IList<dynamic> SelectedObjects { get; set; } = new List<dynamic>();
        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        [Parameter] 
        public IList<dynamic> SelectedItems { get; set; }
        private ListViewModel ListViewModel { get; set; }

        [Parameter]
        public string ViewdefName { get; set; }

        [Parameter]
        public string DefaultViewdefName { get; set; } = "list";

        [Parameter]
        public Action<string> OnClickEdit { get; set; } = null;

        [Parameter]
        public Action<string> OnClickDetail { get; set; } = null;

        [Parameter]
        public Action<string, string> OnClickDelete { get; set; } = null;

        [Parameter]
        public Action OnClickNew { get; set; } = null;

        [Parameter]
        public Action<IList<dynamic>> OnSelectedRow { get; set; } = null;

        [Parameter]
        public IEnumerable<object> Data { get; set; } = null;

        [Parameter]
        public bool IsMultiple { get; set; } = false;
        [Parameter]
        public string ResourceTag { get; set; }
        private IEnumerable<object> data;

        private bool HasCustomActions { get; set; } = false;
        private List<dynamic> CustomActionIcons { get; set; } = new List<dynamic>();
        private List<Button> CustomActions { get; set; }
        private string WithActions {get; set;} = "120px";
        int count;
        private bool HasExtraButtons { get; set; }
        private List<Button> ExtraButtons { get; set; }
        private Button CreateButton { get; set; }
        public RadzenDataGrid<object> _gridRef;
        private FlexComponent _flexComponentRef;
        private bool _isSdkFlex;
        /// <summary>
        /// Gets or sets the fields hidden.
        /// </summary>
        public List<FieldOptions> FieldsHidden { get; set; } = new List<FieldOptions>();

        public List<FieldOptions> SavedHiddenFields { get; set; } = new List<FieldOptions>();
        public List<string> FieldsHiddenList { get; set; } = new List<string>();
        public string BLEntityName { get; set; }

        public string LastFilter { get; set; }
        public bool HasSearchViewdef { get; set; }

        public string FinalViewdefName { get; set; }

        private List<string> _extraFields = new List<string>();

        private bool CanCreate;
        private bool CanEdit;
        private bool CanDelete;
        private bool CanDetail;
        private bool CanAccess;
        private bool CanImport;
        private bool CanExport;
        private string defaultStyleSearchForm = "search_back position-relative px-3";
        private string StyleSearchForm { get; set; } = "search_back position-relative px-3";
        private Radzen.DataGridSelectionMode SelectionMode { get; set; } = Radzen.DataGridSelectionMode.Single;
        Guid needUpdate;
        private string _base_filter = "";
        private bool ErroInAction;

        public dynamic BusinessObjNullable { get; set; }

        private void OnSelectionChanged(IList<object> objects)
        {
            if (OnSelectedRow != null)
            {
                SelectedObjects = objects;
                OnSelectedRow(SelectedObjects);
            }
        }

        private string GetViewdef(string businessName)
        {
            var viewdef = "";
            if (String.IsNullOrEmpty(ViewdefName))
            {
                viewdef = DefaultViewdefName;
            }
            else
            {
                viewdef = ViewdefName;
            }
            FinalViewdefName = viewdef;
            var data = BackendRouterService.GetViewdef(businessName, viewdef);
            if (String.IsNullOrEmpty(data) && viewdef != DefaultViewdefName)
            {
                data = BackendRouterService.GetViewdef(businessName, DefaultViewdefName);
                FinalViewdefName = DefaultViewdefName;
            }
            return data;
        }

        protected async void InitView(string bName = null)
        {
            Loading = true;
            _isEditingFlex = false;
            if (bName == null)
            {
                bName = BusinessName;
            }
            if(bName.Equals("BLFlexProduct", StringComparison.Ordinal)){
                _isSdkFlex = true;
            }
            if (ResourceTag == null)
            {
                ResourceTag = $"{BusinessName}.Plural";
            }
            await CheckPermissions().ConfigureAwait(true);
            var metadata = GetViewdef(bName);
            if (string.IsNullOrEmpty(metadata))
            {
                //ErrorMsg = "No hay definición para la vista de lista";
                ErrorMsg = "Custom.Generic.ViewdefNotFound";
                ErrorList.Add(new ModelMessagesDTO()
                {
                    Message = "Custom.Generic.ViewdefNotFound"
                });
            }
            else
            {
                if (ShowSearchForm)
                {
                    try
                    {   
                        var searchMetadata = BackendRouterService.GetViewdef(bName, "search");
                        HasSearchViewdef = !String.IsNullOrEmpty(searchMetadata);
                        ShowList = !HasSearchViewdef;

                        if (!string.IsNullOrEmpty(searchMetadata))
                        {
                            var searchForm = JsonConvert.DeserializeObject<FormViewModel>(searchMetadata);
                            if (searchForm != null)
                            {
                                foreach (var panel in searchForm.Panels)
                                {
                                    foreach (var field in panel.Fields)
                                    {
                                        field.GetFieldObj(BusinessObj);
                                    }
                                }

                                FieldsHidden = searchForm.Panels.SelectMany(x => x.Fields).Where(x => x.Hidden).ToList();
                            }
                            else
                            {
                                FieldsHidden = new List<FieldOptions>();
                            }
                            _isSearchOpen = true;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        ShowList = true;
                        FieldsHidden = new List<FieldOptions>();
                        string stringError = $"{ex.Message} {ex.StackTrace}";
                        ErrorList.Add(new ModelMessagesDTO()
                        {
                            Message = "Custom.Generic.Message.Error",
                            StackTrace = stringError
                        });
                    }

                    try
                    {
                        SavedHiddenFields = await localStorageService.GetItemAsync<List<FieldOptions>>($"{BusinessName}.Search.HiddenFields").ConfigureAwait(true);
                    }
                    catch (System.Exception ex)
                    {
                        string stringError = $"{ex.Message} {ex.StackTrace}";
                        ErrorList.Add(new ModelMessagesDTO()
                        {
                            Message = "Custom.Generic.Message.Error",
                            StackTrace = stringError
                        });

                    }

                }
                ListViewModel = JsonConvert.DeserializeObject<ListViewModel>(metadata);

                var defaultFields = ListViewModel.Fields.Where(f=> f.CustomComponent == null && f.Name.StartsWith("BaseObj.")).Select(f => f.Name).ToList();
                
                if (ListViewModel.ExtraFields.Count > 0)
                {   
                    _extraFields =  ListViewModel.ExtraFields.Select(f => f)
                    .Union(defaultFields)
                    .ToList();

                    _extraFields = _extraFields.Select(field => field.Replace("BaseObj.", "")).ToList();
                }
                else
                {
                    _extraFields = defaultFields.Select(field => field.Replace("BaseObj.", "")).ToList();
                }

                if(ListViewModel.Buttons != null && ListViewModel.Buttons.Count > 0){
                    var showButton = false;
                    ExtraButtons = new List<Button>();
                    foreach (var button in ListViewModel.Buttons){
                        if(button.ListPermission != null && button.ListPermission.Count > 0){
                            showButton = CheckPermissionsButton(button.ListPermission);
                        }else{
                            showButton = true;
                        }
                        if(showButton){
                            if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-disabled")){
                                var disabled = await evaluateCodeButtons(button, "sdk-disabled");
                                button.Disabled = disabled;
                            }
                            if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-hide")){
                                var hidden = await evaluateCodeButtons(button, "sdk-hide");
                                button.Hidden= hidden;
                            }
                            if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-show")){
                                var show = await evaluateCodeButtons(button, "sdk-show");
                                button.Hidden= !show;
                            }
                            if(button.Id != null){
                                if(Enum.TryParse<enumTypeButton>(button.Id, out enumTypeButton typeButton)){
                                    
                                    switch (typeButton)
                                    {
                                        case enumTypeButton.Create:
                                            CreateButton = button;
                                            break;     
                                        default:
                                            break;
                                    }
                                }
                            }else{
                                ExtraButtons.Add(button);
                            }
                        }
                    }
                    HasExtraButtons = ExtraButtons.Any(x => string.IsNullOrEmpty(x.Id));
                }
                UseFlex = ListViewModel.UseFlex;
                FlexTake = ListViewModel.FlexTake;
                ShowLinkTo = ListViewModel.ShowLinkTo;
                ServerPaginationFlex = ListViewModel.ServerPaginationFlex;
                _showActions = ListViewModel.ShowActions;
                if(ShowActions != null && ViewdefName == null){
                    _showActions = ShowActions.Value;
                }
                if(ListViewModel.AllowEdit != null){
                    AllowEdit = ListViewModel.AllowEdit.Value;
                }
                if(ListViewModel.AllowDelete != null){
                    AllowDelete = ListViewModel.AllowDelete.Value;
                }
                if(ListViewModel.AllowDetail != null){
                    AllowDetail = ListViewModel.AllowDetail.Value;
                }
                if(ListViewModel.AllowCreate != null){
                    AllowCreate = ListViewModel.AllowCreate.Value;
                }
                if(ListViewModel.AllowExport != null){
                    AllowExport = ListViewModel.AllowExport.Value;
                }
                if(ListViewModel.ResourceTag != null){
                    ResourceTag = ListViewModel.ResourceTag;
                }
                if(ListViewModel.RedirectDetail != null){
                    RedirectDetail = ListViewModel.RedirectDetail.Value;
                }
                if(ListViewModel.RedirectCreate != null){
                    RedirectCreate = ListViewModel.RedirectCreate.Value;
                }
                //TODO: quitar cuando se pueda usar flex en los custom components
                var fieldsCustomComponent = ListViewModel.Fields.Where(x => x.CustomComponent != null).ToList();
                if(fieldsCustomComponent.Count > 0){
                    UseFlex = false;
                }
                if(ListViewModel.CustomActions != null && ListViewModel.CustomActions.Count > 0){
                    var showButton = false;
                    CustomActions = new List<Button>();
                    foreach (var button in ListViewModel.CustomActions){
                        if(button.ListPermission != null && button.ListPermission.Count > 0){
                            showButton = CheckPermissionsButton(button.ListPermission);
                        }else{
                            showButton = true;
                        }
                        if(showButton){
                            CustomActions.Add(button);
                        }
                    }
                    if(CustomActions.Count > 0){
                        var withInt = (CustomActions.Count+2)*40;
                        WithActions = $"{withInt}px";
                        CustomActionIcons = new List<dynamic>();
                        foreach (var action in CustomActions)
                        {
                            var obj = new{
                                icon = action.IconClass,
                                disabled = action.Disabled,
                                hide = action.Hidden
                            };
                            CustomActionIcons.Add(obj);
                        }
                        HasCustomActions = true;
                    }
                }else{
                    CustomActionIcons = new List<dynamic>();
                    HasCustomActions = false;
                }
                foreach (var field in ListViewModel.Fields)
                {
                    field.GetFieldObj(BusinessObj);
                }
                if(ListViewModel.Filters != null && ListViewModel.Filters.Count > 0){
                    _base_filter = await FormUtils.GenerateFilters(ListViewModel.Filters, BusinessObj);
                }
            }
            data = null;
            Loading = false;
            if (BusinessObj != null && BusinessObj.BaseObj != null)
            {
                BLEntityName = BusinessObj.BaseObj.GetType().Name;
            }
            BusinessObj.ParentComponent = this;
            
            hideCustomColumn();
            FilterFlex = GetFilters(_base_filter);
            StateHasChanged();

        }

        public async Task Refresh(bool Reload = false)
        {
            if (Reload)
            {
                Restart();
            }
            hideCustomColumn();
            StateHasChanged();
        }

        private bool CheckPermissionsButton(List<int> ListPermission){
            var showButton = false;
            if(FeaturePermissionService != null){
                try{
                    showButton = FeaturePermissionService.CheckUserActionPermissions(BusinessName, ListPermission, AuthenticationService);
                }catch(System.Exception ex)
                {
                    string stringError = $"{ex.Message} {ex.StackTrace}";
                    ErrorList.Add(new ModelMessagesDTO()
                    {
                        Message = "Custom.Generic.Message.Error",
                        StackTrace = stringError
                    });

                }
            }
            return showButton;
        }

        private async Task CheckPermissions()
        {
            if (FeaturePermissionService != null && !string.IsNullOrEmpty(BusinessName))
            {
               
               if(IsSubpanel && BusinessName.Equals("BLAttachmentDetail",StringComparison.Ordinal))
               {
                    try
                    {
                        CanAccess = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.AccessAttachment, AuthenticationService).ConfigureAwait(true);
                        CanCreate = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.UploadAttachment, AuthenticationService).ConfigureAwait(true);
                        CanDelete = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.DeleteAttachment, AuthenticationService).ConfigureAwait(true);
                        CanDetail = await FeaturePermissionService.CheckUserActionPermission(BLNameParentAttatchment, enumSDKActions.DownloadAttachment, AuthenticationService).ConfigureAwait(true);
                    }
                    catch (System.Exception ex)
                    {
                        string stringError = $"{ex.Message} {ex.StackTrace}";
                        ErrorList.Add(new ModelMessagesDTO()
                        {
                            Message = "Custom.Generic.Message.Error",
                            StackTrace = stringError
                        });
                    }
               }else
               {
                    try
                    {
                        string businessName = BusinessName;
                        if(businessName.Equals("BLFlexProduct", StringComparison.Ordinal)){
                            businessName = "BLFlex";
                        }
                        CanAccess = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Access, AuthenticationService).ConfigureAwait(true);
                        CanCreate = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Create, AuthenticationService).ConfigureAwait(true);
                        CanEdit = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Edit, AuthenticationService).ConfigureAwait(true);
                        CanDelete = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Delete, AuthenticationService).ConfigureAwait(true);
                        CanDetail = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Detail, AuthenticationService).ConfigureAwait(true);
                        CanImport = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Import, AuthenticationService).ConfigureAwait(true);
                        CanExport = await FeaturePermissionService.CheckUserActionPermission(businessName, enumSDKActions.Export, AuthenticationService).ConfigureAwait(true);
                    }
                    catch (System.Exception ex)
                    {
                        string stringError = $"{ex.Message} {ex.StackTrace}";
                        ErrorList.Add(new ModelMessagesDTO()
                        {
                            Message = "Custom.Generic.Message.Error",
                            StackTrace = stringError
                        });
                    }
               }

                if (!CanAccess && !FromEntityField)
                {
                    ErrorMsg = "Custom.Generic.Unauthorized";
                    ErrorList.Add(new ModelMessagesDTO()
                    {
                        Message = "Custom.Generic.Unauthorized"
                    });
                    _ = NotificationService.ShowError("Custom.Generic.Unauthorized");
                    if(!IsSubpanel){
                        // NavigationService.NavigateTo("/", replace: true);
                    }
                }

            }

        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (IsMultiple){
                SelectionMode = Radzen.DataGridSelectionMode.Multiple;
            }

            //Restart();
        }

        public async Task<bool> evaluateCodeButtons(Button button, string condition){
            bool disabled = button.Disabled;
            var sdkDisable = button.CustomAttributes[condition];
            if(sdkDisable != null){
                var eject = await Evaluator.EvaluateCode(sdkDisable.ToString(), BusinessObj); //revisar
                if(eject != null){
                    disabled = (bool)eject;
                }
            }
            return disabled;
        }
        

        private object SetValuesObjToNullable(Type newType, dynamic originalObj, dynamic instanceBase = null){
            var instance = Activator.CreateInstance(newType);
            var originalProperties = originalObj.GetType().GetProperties();
            var newProperties = newType.GetProperties();

            foreach (var originalProperty in originalProperties){
                var newProperty = newProperties.FirstOrDefault(p => p.Name == originalProperty.Name);
                if (newProperty != null)
                {
                    var originalValue = originalProperty.GetValue(originalObj);
                    if (originalValue != null && originalProperty.Name == "BaseObj"){
                        newProperty.SetValue(instance, instanceBase);
                    }else{
                        newProperty.SetValue(instance, originalValue);
                    }
                }
            }

            return instance;
        }

        public Type CreateNullableType(Type originalType, bool includeNonNullable = false, Type baseType = null)
        {
            // Crea una nueva assembly dinámica.
            AssemblyName assemblyName = new AssemblyName("DynamicAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            
            // Crea un nuevo módulo en la assembly dinámica.
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
            
            // Crea un nuevo tipo basado en el tipo original con propiedades nulas.
            TypeBuilder typeBuilder = moduleBuilder.DefineType($"{originalType.Name}", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, typeof(object));
            
            foreach (var property in originalType.GetProperties())
            {   
                Type newType = null;
                Type propertyType = property.PropertyType;
                if(property.Name.Equals("BaseObj")){
                    propertyType = baseType;
                }

                bool propertyNullable = propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

                if(includeNonNullable && !propertyNullable){
                // Obtiene el tipo anulable correspondiente a la propiedad.
                    Type nullableType = Nullable.GetUnderlyingType(propertyType);
                    if (nullableType == null && propertyType.IsValueType)
                    {
                        nullableType = typeof(Nullable<>).MakeGenericType(propertyType);
                    }
                    else if (nullableType == null)
                    {
                        nullableType = propertyType;
                    }
                    newType = nullableType;
                }else{
                    newType = propertyType;
                }
                // Crea un campo privado para cada propiedad del tipo original.
                FieldBuilder fieldBuilder = typeBuilder.DefineField($"_{property.Name}", newType, FieldAttributes.Private);
                
                // Crea una propiedad pública con el mismo nombre y tipo, pero con un tipo anulable.
                PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(property.Name, PropertyAttributes.None, newType, new Type[] { newType });
                MethodBuilder getMethodBuilder = typeBuilder.DefineMethod($"get_{property.Name}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, newType, Type.EmptyTypes);
                ILGenerator getIL = getMethodBuilder.GetILGenerator();
                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getIL.Emit(OpCodes.Ret);
                propertyBuilder.SetGetMethod(getMethodBuilder);

                // Crea el método set para la propiedad
                MethodBuilder setMethodBuilder = typeBuilder.DefineMethod($"set_{property.Name}", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(void), new Type[] { newType });
                ILGenerator setIL = setMethodBuilder.GetILGenerator();
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Stfld, fieldBuilder);
                setIL.Emit(OpCodes.Ret);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }
            
            // Crea el constructor por defecto.
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            ILGenerator constructorIL = constructorBuilder.GetILGenerator();
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            constructorIL.Emit(OpCodes.Ret);
            
            // Crea el tipo y lo devuelve.
            Type nullType = typeBuilder.CreateType();
            return nullType;
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            bool shouldRestart = validateChanged(parameters);
            await base.SetParametersAsync(parameters);
            if(shouldRestart){
                Restart();
            }
        }        
        private bool validateChanged(ParameterView parameters)
        {
            var type = this.GetType();
            var properties = type.GetProperties();
            var result = false;

            foreach (var property in properties){
                var HasCustomAttributes = property.GetCustomAttributes().Count() > 0;
                if(!HasCustomAttributes){
                    continue;
                }
                var dataAnnotationProperty = property.GetCustomAttributes().First().GetType();
                var parameterType = typeof(ParameterAttribute);
                if(dataAnnotationProperty == parameterType){
                    try{
                        result = CompareValues(parameters, property);
                        if(result){                            
                            break;
                        }                        
                    }catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return result;
        }

        private bool CompareValues(ParameterView parameters, PropertyInfo property)
        {
            bool result = false;
            Type typeProp = property.PropertyType;

            if (typeProp == typeof(string))
            {
                parameters.TryGetValue<string>(property.Name, out var value);
                var valueProperty = property.GetValue(this, null);
                if(value != null){
                    result = !value.Equals(valueProperty);
                }
            }else if (typeProp.IsGenericType && typeProp.GetGenericTypeDefinition() == typeof(List<>))
            {
                parameters.TryGetValue<IEnumerable>(property.Name, out var value);
                var valueProperty = property.GetValue(this, null);
                if(value != null){
                    result = !(value as IEnumerable<object>)?.SequenceEqual(valueProperty as IEnumerable<object>) ?? false;
                }
            }
            else
            {
                parameters.TryGetValue<string>(property.Name, out var value);
                var valueProperty = property.GetValue(this, null);
                if(value != null){
                    result = value != valueProperty;
                }
            }

            return result;
        }

        private void initNullable(){
            Type newTypeBase = CreateNullableType(BusinessObj.BaseObj.GetType(), true);
            var instanceBase = Activator.CreateInstance(newTypeBase);

            Type newType = CreateNullableType(BusinessObj.GetType(), false, newTypeBase);
            var instance = SetValuesObjToNullable(newType, BusinessObj, instanceBase);

            BusinessObjNullable = instance;
        }

        private void Restart()
        {
            guidListView = Guid.NewGuid().ToString().Replace("-", "", StringComparison.Ordinal);
            Loading = false;
            ResourceTag = null;
            ErrorMsg = "";
            ErrorList = new();
            _extraFields = new List<string>();
            InitView();
            if(ShowSearchForm && HasSearchViewdef){
                initNullable();
            }
            data = null;
            if (Data != null)
            {
                data = Data;
                count = data.Count();
            }
            if (_gridRef != null || SearchFormRef != null)
            {
                needUpdate = Guid.NewGuid();
                if (_gridRef != null)
                {
                    _gridRef.Reload();
                }
                if (SearchFormRef != null)
                {
                    StyleSearchForm = defaultStyleSearchForm;
                }
            }
            StateHasChanged();
        }

        private string GetFormKey()
        {
            return $"{needUpdate.ToString()}-search";
        }

        private string GetFilters(string base_filter = "")
        {
            var filters = $"{base_filter}";
            try
            {
                Type type = BusinessObj.BaseObj.GetType();
                bool containsRowidCompany = type.GetProperties().Any(x => x.Name == "RowidCompany");
                if (containsRowidCompany && BusinessObj.BaseObj.RowidCompany != 0)
                {
                    if (Utilities.IsAssignableToGenericType(type, typeof(BaseCompany<>)))
                    {
                        if (!string.IsNullOrEmpty(filters))
                        {
                            filters += " && ";
                        }
                        if (BusinessObj?.BaseObj != null)
                        {
                            filters += $"RowidCompany={BusinessObj.BaseObj.RowidCompany}";
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                string stringError = $"{ex.Message} {ex.StackTrace}";
                ErrorList.Add(new ModelMessagesDTO()
                {
                    Message = "Custom.Generic.Message.Error",
                    StackTrace = stringError
                });
            }
            if (ConstantFilters != null)
            {
                foreach (var filter in ConstantFilters)
                {
                    if (!string.IsNullOrEmpty(filters))
                    {
                        filters += " && ";
                    }
                    filters += $"{filter}";
                }
            }

            try
            {
                if (SearchFormRef != null && SearchFormRef.BusinessName == BusinessName)
                {
                    var searchFields = SearchFormRef.GetFields();
                    foreach (var field in searchFields)
                    {
                        var tmpFilter = "";
                        if (field.FiltersCustom != null && field.FiltersCustom.Count > 0)
                        {
                            foreach (string filterName in field.FiltersCustom)
                            {
                                FieldOptions fieldOptions = new FieldOptions()
                                {
                                    Name = filterName,
                                    PropertyName = filterName.Replace("BaseObj.", "", StringComparison.Ordinal)
                                };
                                FieldObj fieldFilter = fieldOptions.GetFieldObj(BusinessObjNullable);
                                SetTmpFilter(fieldFilter, fieldOptions, ref tmpFilter);
                            }
                        }
                        else
                        {
                            var fieldObj = field.GetFieldObj(BusinessObjNullable);
                            SetTmpFilter(fieldObj, field, ref tmpFilter);
                        }
                        if (!string.IsNullOrEmpty(tmpFilter))
                        {
                            if (!string.IsNullOrEmpty(filters))
                            {
                                filters += " && ";
                            }
                            filters += $"({tmpFilter})";
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                string stringError = $"{ex.Message} {ex.StackTrace}";
                ErrorList.Add(new ModelMessagesDTO()
                {
                    Message = "Custom.Generic.Message.Error",
                    StackTrace = stringError
                });
            }

            return filters;
        }

        private static void SetTmpFilter(dynamic fieldObj, FieldOptions field, ref string tmpFilter)
        {
            if (fieldObj != null)
            {
                string fieldName = field.Name.Replace("BaseObj.", "", StringComparison.Ordinal);
                dynamic searchValue = GetSearchValue(fieldObj);
                if (searchValue == null)
                {
                    return;
                }

                //check if searchValue is string and an empty string
                if (searchValue is string && string.IsNullOrEmpty(searchValue))
                {
                    return;
                }

                switch (fieldObj.FieldType)
                {
                    case FieldTypes.CharField:
                    case FieldTypes.TextField:
                        tmpFilter =
                            $"({fieldName} == null ? \"\" : {fieldName}).ToLower().Contains(\"{searchValue}\".ToLower())";
                        break;
                    case FieldTypes.IntegerField:
                    case FieldTypes.DecimalField:
                    case FieldTypes.SmallIntegerField:
                    case FieldTypes.BigIntegerField:
                    case FieldTypes.ByteField:
                        if (!fieldObj.IsNullable)
                        {
                            tmpFilter = $"{fieldName} == {searchValue}";
                        }
                        else
                        {
                            tmpFilter = $"({fieldName} == null ? 0 : {fieldName}) == {searchValue}";
                        }

                        break;
                    case FieldTypes.BooleanField:
                        if (!searchValue)
                        {
                            break;
                        }

                        if (!fieldObj.IsNullable)
                        {
                            tmpFilter = $"{fieldName} == {searchValue}";
                        }
                        else
                        {
                            tmpFilter = $"({fieldName} == null ? false : {fieldName}) == {searchValue}";
                        }

                        break;
                    case FieldTypes.DateField:
                    case FieldTypes.DateTimeField:

                        DateTime selectedDate = searchValue;
                        DateTime endDate = selectedDate.AddDays(1).AddSeconds(-1);
                        
                        if (!fieldObj.IsNullable)
                        {
                            tmpFilter = $"{fieldName} >= DateTime.Parse(\"{selectedDate:yyyy-MM-dd}\") && {fieldName} <= DateTime.Parse(\"{endDate:yyyy-MM-dd HH:mm:ss}\")";
                        }
                        else
                        {
                            tmpFilter =
                                $"({fieldName} == null ? DateTime.MinValue : {fieldName}) >= DateTime.Parse(\"{selectedDate:yyyy-MM-dd}\") && " +
                                $"({fieldName} == null ? DateTime.MinValue : {fieldName}) <= DateTime.Parse(\"{endDate:yyyy-MM-dd HH:mm:ss}\")";
                        }
                        break;

                    case FieldTypes.EntityField:
                        if (!fieldObj.IsNullable)
                        {
                            if (searchValue.Rowid != 0)
                            {
                                tmpFilter = $"({fieldName}.Rowid) == {searchValue.Rowid}";
                            }
                        }
                        else
                        {
                            tmpFilter = $"({fieldName} == null ? 0 : {fieldName}.Rowid) == {searchValue.Rowid}";
                        }

                        break;

                    case FieldTypes.Custom:
                    case FieldTypes.SelectField:
                        if (field.CustomType == "SelectBarField" || field.FieldType == FieldTypes.SelectField)
                        {
                            Type enumType = searchValue.GetType();
                            var EnumValues = Enum.GetValues(enumType);
                            var LastValue = EnumValues.GetValue(EnumValues.Length - 1);

                            if (Convert.ToInt32(LastValue, CultureInfo.InvariantCulture) + 1 != Convert.ToInt32(searchValue))
                            {
                                tmpFilter = $"{fieldName} == {Convert.ToInt32(searchValue)}";
                            }
                        }

                        break;
                }
            }
        }

        private static dynamic GetSearchValue(dynamic fieldObj)
        {
            return fieldObj.ModelObj.GetType().GetProperty(fieldObj.Name)?.GetValue(fieldObj.ModelObj, null);
        }

        async Task LoadData(LoadDataArgs args)
        {
            if (Data != null)
            {
                data = Data;
                count = data.Count();
                LoadingData = false;
                StateHasChanged();
                return;
            }
            if (!ListViewModel.InfiniteScroll)
            {
                data = null;
            }
            if (data == null)
            {
                LoadingData = true;
            }
            var filters = GetFilters(args.Filter);
            if (LastFilter != filters)
            {
                LastFilter = filters;
                LoadingData = true;
                data = null;
            }

            FilterFlex = filters;
            bool includeCount = false;
            if(!UseFlex){
                includeCount = true;
            }
        var dbData = await BusinessObj.GetDataAsync(args.Skip, args.Top, filters, args.OrderBy, includeCount, _extraFields);
            if(dbData.Errors != null && dbData.Errors.Count > 0)
            {
                foreach (var error in dbData.Errors)
                {
                    ErrorList.Add(new ModelMessagesDTO()
                    {
                        Message = error
                    });
                }
            }
            data = dbData.Data;
            count = dbData.TotalCount;
            LoadingData = false;
        }

        private void GoToEdit(Int64 id)
        {
            if (OnClickEdit != null)
            {
                OnClickEdit(id.ToString());
            }
            else
            {
                if(BusinessName.Equals("BLFlexProduct", StringComparison.Ordinal)){
                    NavManager.NavigateTo($"/BLFlex/Render/?currentView=edit&flexRowid={id}&is_product=true", replace : true);
                }else{
                    NavManager.NavigateTo($"{BusinessName}/edit/{id}");
                }
            }

        }
        private void GoToCreate()
        {
            if (OnClickNew != null)
            {
                OnClickNew();
            }
            else
            {
                NavManager.NavigateTo($"{BusinessName}/create/");
            }
        }

        private void GoToImport()
        {
            NavManager.NavigateTo($"{BusinessName}/Import/");
        }
        
        private async Task GoToExport()
        {
            string resourceTag = ResourceTag;
            string resourceName = await UtilsManager.GetResource(resourceTag).ConfigureAwait(true);
            await JSRuntime.InvokeAsync<object>("oreports_app_table_flexdebug_"+guidListView+".exportToExcel", resourceName);
        }

        private void GoToDetail(Int64 id)
        {
            if (OnClickDetail != null)
            {
                OnClickDetail(id.ToString());
            }
            else
            {
                if(BusinessName.Equals("BLFlexProduct", StringComparison.Ordinal)){
                    NavManager.NavigateTo($"/BLFlex/Render/?currentView=detail&flexRowid={id}&is_product=true", replace : true);
                }else{
                    NavManager.NavigateTo($"{BusinessName}/detail/{id}/");
                }
            }
        }
        [JSInvokable]
        public async Task EditFromReact(Int64 id)
        {
            GoToEdit(id);
        }

        [JSInvokable]
        public async Task DetailFromReact(Int64 id)
        {
            GoToDetail(id);
        }

        [JSInvokable]
        public async Task<bool> DeleteFromReact(Int64 id, string object_string)
        {
            if (OnClickDelete != null){
                OnClickDelete(id.ToString(), object_string);
            }
            if (UseFlex && !IsSubpanel)
            {
                var confirm = await ConfirmDelete();
                SDKGlobalLoaderService.Show();
                if (confirm){
                    BusinessObj.BaseObj.Rowid = Convert.ChangeType(id, BusinessObj.BaseObj.Rowid.GetType());
                    DeleteBusinessObjResponse result = await BusinessObj.DeleteAsync();
                    SDKGlobalLoaderService.Hide();
                    if (result != null && result.Errors.Count == 0){
                        return true;
                    }else
                    {
                        _ = NotificationService.ShowError("Custom.Generic.Message.Error");
                        foreach (var error in result.Errors)
                        {
                            if (error.Format != null && error.Format.Any())
                            {
                               
                                ErrorList.Add(new ModelMessagesDTO()
                                {
                                     MessageFormat = new Dictionary<string, List<string>>()
                                    {
                                        { error.Message, error.Format.ToList() }
                                    },
                                });
                            }else
                            {
                                ErrorList.Add(new ModelMessagesDTO()
                                {
                                    Message = error.Message
                                });
                            }
                        }
                        ErroInAction = true;
                        _ = NotificationService.ShowError("Custom.Generic.Message.DeleteError");
                        StateHasChanged();                        
                    }
                }

                SDKGlobalLoaderService.Hide();
            }
            return false;
        }

        [JSInvokable]
        public async Task<bool> CustomActionFromReact(Int64 indexButton, object rowid)
        {
            Button button = CustomActions[(int)indexButton];
            if(button != null){
                var bl = BackendRouterService.GetSDKBusinessModel(BusinessName, AuthenticationService);
                var result = await bl.Call("DataEntity", rowid.ToString());
                if(result.Success){
                    var obj = result.Data;
                    OnClickCustomAction(button, obj);
                    return true;
                }
            }
            return false;
        }

        [JSInvokable]
        public async Task OnSelectFromReact(string item){
            if(string.IsNullOrEmpty(item)){
                return;
            }
            List<dynamic> objects = JsonConvert.DeserializeObject<List<dynamic>>(item);
            IList<dynamic> list = new List<dynamic>();
            foreach (var dynamicObj in objects)
            {
                dynamic obj = Activator.CreateInstance(BusinessObj.BaseObj.GetType());
                foreach(var prop in dynamicObj){
                    var propertyName = prop.Name;
                    if(propertyName.Equals("rowid")){
                        propertyName = "Rowid";
                    }
                    await SetValueObj(obj, propertyName, prop.Value);
                }
                list.Add(obj);
            }
            OnSelectionChanged(list);
        }

        [JSInvokable]
        public async Task<bool> DisableActionFromReact(string dataStr, Int64 index){
            Button button = CustomActions[(int)index];
            bool res = false;
            if(button != null){
                var data = JsonConvert.DeserializeObject<dynamic>(dataStr);
                dynamic disableCondition = null;
                if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-disabled")){
                    var sdkDisable = button.CustomAttributes["sdk-disabled"];
                    if(sdkDisable != null){
                        disableCondition = sdkDisable;
                    }
                }
                if(disableCondition != null){
                    res = await EvaluateCondition(data, disableCondition);
                }
            }
            return res;
        }

        [JSInvokable]
        public async Task<bool> HideActionFromReact(string dataStr, Int64 index){
            Button button = CustomActions[(int)index];
            bool res = false;
            bool deny = false;
            if(button != null){
                var data = JsonConvert.DeserializeObject<dynamic>(dataStr);
                dynamic hideCondition = null;
                if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-hide") && button.CustomAttributes["sdk-hide"] != null){
                    hideCondition = button.CustomAttributes["sdk-hide"];
                }else if(button.CustomAttributes != null && button.CustomAttributes.ContainsKey("sdk-show") && button.CustomAttributes["sdk-show"] != null){
                    hideCondition = button.CustomAttributes["sdk-show"];
                    deny = true;
                }
                if(hideCondition != null){
                    res = await EvaluateCondition(data, hideCondition);
                    if(deny){
                        res = !res;
                    }
                }
            }
            return res;
        }

        private async Task<bool> EvaluateCondition(dynamic data, dynamic condition){
            bool res = false;
            data = JsonConvert.DeserializeObject(data.ToString());
            dynamic obj = Activator.CreateInstance(BusinessObj.BaseObj.GetType());
            if(condition is bool){
                res = (bool)condition;
            }else{
                var eject = await Evaluator.EvaluateCode(condition, BusinessObj);
                if(eject != null){
                    if(eject is bool){
                        res = (bool)eject;
                    }else{
                        foreach(var prop in data){
                            var propertyName = prop.Name;
                            if(propertyName.Equals("rowid")){
                                propertyName = "Rowid";
                            }
                            await SetValueObj(obj, propertyName, prop.Value);
                        }
                        MethodInfo methodInfo = (MethodInfo)(eject.GetType().GetProperty("Method").GetValue(eject));
                        if(methodInfo != null){
                            if(methodInfo.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false).Length > 0){
                                res = await eject(obj);
                            }else{
                                res = eject(obj);
                            }
                        }
                    }
                }
            }
            return res;
        }

        private async Task SetValueObj(dynamic obj, string propertyName, dynamic value)
        {
            var type = obj.GetType();
            var propertyNameSplit = propertyName.Split('.');
            string propertyNameAux = propertyNameSplit[0];
            var property = type.GetProperty(propertyNameAux);
            if (property != null){
                for (int i = 0; i < propertyNameSplit.Length; i++){
                    var propertyType = property.PropertyType;
                    if(i == propertyNameSplit.Length - 1){
                        var val = value;
                        bool isRelation = propertyType != null && propertyType.IsClass && propertyType != typeof(string);
                        if(propertyType.IsEnum){
                            var enumType = propertyType;
                            var EnumValues = Enum.GetValues(enumType);
                            foreach(var enumValue in EnumValues){
                                var tagEnum = $"Enum.{enumType.Name}.{enumValue.ToString()}";
                                var resource = await UtilsManager.GetResource(tagEnum);
                                if(resource.ToString().Equals(value.ToString())){
                                    val = enumValue;
                                    break;
                                }
                            }
                            if(val != null && !val.ToString().Equals(value.ToString())){
                                property.SetValue(obj, val);
                            }
                        }else if(!isRelation){
                            val = Convert.ChangeType(value, propertyType);
                            property.SetValue(obj, val);
                        }else{
                            continue;
                        }
                    }else{
                        var objProp = property.GetValue(obj);
                        if(objProp == null){
                            objProp = Activator.CreateInstance(propertyType);
                        }
                        propertyNameAux = propertyNameSplit[i+1];
                        await SetValueObj(objProp, propertyNameAux, value);
                        property.SetValue(obj, objProp);
                        break;
                    }
                }
            }
        }

        private async Task GoToDelete(Int64 id, string object_string)
        {
            if (OnClickDelete != null){
                OnClickDelete(id.ToString(), object_string);
            }
        }

        private void GoToEditFlex(){
            SetSearchFromVisibility(true);
            _isEditingFlex = true;
            JSRuntime.InvokeAsync<object>("oreports_app_flexdebug_"+guidListView+".props.setEditListFlex");
        }

        private void CancelEdit(){
            _isEditingFlex = false;
            JSRuntime.InvokeAsync<object>("oreports_app_flexdebug_"+guidListView+".props.setEditListFlex");
        }

        private void SaveAndCloseList(){
            _isEditingFlex = false;
            JSRuntime.InvokeAsync<object>("oreports_app_flexdebug_"+guidListView+".props.save");
            JSRuntime.InvokeAsync<object>("oreports_app_flexdebug_"+guidListView+".props.setEditListFlex");
        }

        private async Task OnClickSearch()
        {
            LoadingSearch = true;
            LoadingData = true;
            data = null;
            var filters = GetFilters(_base_filter);
            if(ServerPaginationFlex && UseFlex){
                Data = await BusinessObj.GetDataWithTop(filters);
                 if (Data != null && Data.Count() == 1 && RedirectDetail){
                     if (!FromEntityField)
                     { 
                         GoToDetail((dynamic)Data.First());
                         return;
                     }
                     else
                     {
                         try
                         {
                             long dataRowid = (long)Data.First();
                             var dataObj = await BusinessObj.GetAsync(dataRowid);
                             IList<object> objects = new List<object> { dataObj };
                             OnSelectionChanged(objects);
                             return;
                         }
                         catch (Exception ex)
                         {
                            string stringError = $"{ex.Message} {ex.StackTrace}";
                            ErrorList.Add(new ModelMessagesDTO()
                            {
                                Message = "Custom.Generic.Message.Error",
                                StackTrace = stringError
                            });
                         }
                         
                     }
                 }
                // Data = dbData.Data;
                // if (Data.Count() == 1)
                // {
                //     GoToDetail(((dynamic)Data.First()).Rowid);   
                //     return;
                // }
                // count = dbData.TotalCount;
            }else{
                bool includeCount = false;
                int? skip = null;
                int? take = null;
                if(!UseFlex){
                    includeCount = true;
                    skip = 0;
                    take = ListViewModel.Paging.PageSize;
                    if(_gridRef != null){
                        var currentPage = _gridRef.GetType().GetProperty("CurrentPage").GetValue(_gridRef);
                        skip = (int)currentPage * ListViewModel.Paging.PageSize;
                    }
                }
                var dbData = await BusinessObj.GetDataAsync(skip, take, filters, "", includeCount, _extraFields);
                if(dbData.Errors != null && dbData.Errors.Count > 0)
                {
                    foreach (var error in dbData.Errors)
                    {
                        ErrorList.Add(new ModelMessagesDTO()
                        {
                            Message = error
                        });
                    }
                }
                count = dbData.TotalCount;
                data = dbData.Data;
                if (count == 1 && RedirectDetail){
                    if(!FromEntityField){
                        GoToDetail(((dynamic)data.First()).Rowid);
                    }else{
                        IList<object> objects = new List<object> { data.First()};
                        OnSelectionChanged(objects);
                    }
                    return;
                }

                // if(!UseFlex){
                //     Data = data;
                // }
            }
            LoadingData = false;
            LoadingSearch = false;
            ShowList = true;
            FilterFlex = filters;
            SearchFlex(FilterFlex);
            SetSearchFromVisibility(true);
        }

        private void SearchFlex(string filter)
        {
            if (UseFlex)
            {
                _isSearchOpen = false;
                JSRuntime.InvokeAsync<object>("oreports_app_flexdebug_"+guidListView+".props.setSearchListFlex", filter);
            }
        }

        public void SetSearchFromVisibility(bool hideForm)
        {
            if (hideForm)
            {
                StyleSearchForm = "search_back search_back_hide position-relative";
            }
            else
            {
                StyleSearchForm = defaultStyleSearchForm;
            }
            try
            {
                StateHasChanged();
            }
            catch (System.Exception ex)
            {
                _ = InvokeAsync(() => StateHasChanged());
                string stringError = $"{ex.Message} {ex.StackTrace}";
                ErrorList.Add(new ModelMessagesDTO()
                {
                    Message = "Custom.Generic.Message.Error",
                    StackTrace = stringError
                });
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
                hideCustomColumn();
            }
        }

        private async Task OnClickCustomAction(Button button, dynamic obj)
        {
            if (!string.IsNullOrEmpty(button.Action)){

                var eject = await Evaluator.EvaluateCode(button.Action, BusinessObj);
                MethodInfo methodInfo = (MethodInfo)(eject.GetType().GetProperty("Method").GetValue(eject));
                if(methodInfo != null){
                    if(methodInfo.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false).Length > 0){
                        await eject(obj);
                    }else{
                        eject(obj);
                    }
                }
            }
        }

        private IDictionary<string, object> GetSelectFieldParameters(dynamic data, FieldOptions field, string fieldName)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            if(fieldName.Split(".").Length > 1)
            {
                string[] fieldPath = fieldName.Split('.');
                var typeBaseSDK = typeof(BaseSDK<>);
                object currentData = Utilities.CreateCurrentData(data,fieldPath,typeBaseSDK);
                parameters.Add("BindModel", currentData);
            }else{
                parameters.Add("BindModel", data);
            }
            parameters.Add("FieldName", field.GetFieldObj(data).Name);
            parameters.Add("FieldOpt", field);
            return parameters;
        }

        private void hideCustomColumn()
        {
            FieldsHiddenList = new List<string>();
            var useRoslyn = false;
            if(useRoslyn)
            {
                string code = "";
                for (int i = 0; i < ListViewModel.Fields.Count; i++)
                {
                    var field = ListViewModel.Fields[i];
                    if (field.CustomAttributes != null)
                    {
                        var fieldCustomAttr = field.CustomAttributes;
                        foreach (var CustomAttr in fieldCustomAttr)
                        {
                            if (CustomAttr.Key == "sdk-hide")
                            {
                                try
                                {
                                    code += @$"
                                    try {{ ListViewFields[{i}].Hidden = ({(string)CustomAttr.Value}); }} catch (Exception ex) {{ throw;}}";
                                }
                                catch (Exception ex)
                                {
                                    string stringError = $"{ex.Message} {ex.StackTrace}";
                                    ErrorList.Add(new ModelMessagesDTO()
                                    {
                                        Message = "Custom.Generic.Message.Error",
                                        StackTrace = stringError
                                    });
                                }
                            }
                            if (CustomAttr.Key == "sdk-show")
                            {
                                try
                                {
                                    code += @$"
                                    try {{ ListViewFields[{i}].Hidden = !({(string)CustomAttr.Value}); }} catch (Exception ex) {{ throw;}}";
                                }
                                catch (Exception ex)
                                {
                                    string stringError = $"{ex.Message} {ex.StackTrace}";
                                    ErrorList.Add(new ModelMessagesDTO()
                                    {
                                        Message = "Custom.Generic.Message.Error",
                                        StackTrace = stringError
                                    });
                                }
                            }
                        }
                    }
                }
                if (code != null & code != "")
                {
                    _ = Task.Run(async () =>
                    {
                        BusinessObj.ListViewFields = ListViewModel.Fields;
                        await Evaluator.EvaluateCode(code, BusinessObj);
                        _ = InvokeAsync(() => StateHasChanged());
                    });
                }
            }else{
                List<string> allowAttr = new List<string>(){
                    "sdk-show",
                    "sdk-hide",
                }; //TODO: Enum
                if(ListViewModel != null && ListViewModel.Fields != null)
                {
                    for (int i = 0; i < ListViewModel.Fields.Count; i++) //fix error null
                    {
                        var field = ListViewModel.Fields[i];
                        if (field.CustomAttributes != null)
                        {
                            _ = Task.Run(async () =>
                            {
                                bool shouldUpdate = false;
                                foreach (var attr in field.CustomAttributes)
                                {
                                    if(!allowAttr.Contains(attr.Key))
                                    {
                                        continue;
                                    }
                                    try
                                    {
                                        var result = (bool)await Evaluator.EvaluateCode((string)attr.Value, BusinessObj);
                                        switch (attr.Key)
                                        {
                                            case "sdk-show":
                                                if(field.Hidden != !result)
                                                {
                                                    field.Hidden = !result;
                                                    shouldUpdate = true;
                                                }
                                                break;
                                            case "sdk-hide":
                                                if(field.Hidden != result)
                                                {
                                                    field.Hidden = result;
                                                    shouldUpdate = true;
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    catch (System.Exception ex)
                                    {
                                        string stringError = $"{ex.Message} {ex.StackTrace}";
                                        ErrorList.Add(new ModelMessagesDTO()
                                        {
                                            Message = "Custom.Generic.Message.Error",
                                            StackTrace = stringError
                                        });
                                    }
                                    if(field.Hidden){
                                        FieldsHiddenList.Add(field.Name.Replace("BaseObj.", "", StringComparison.InvariantCultureIgnoreCase));
                                    }
                                }
                                if(shouldUpdate)
                                {
                                    _ = InvokeAsync(() => StateHasChanged());
                                }
                            });
                        }
                    }
                }

            }
            
        }

        protected async Task UpdateSearchForm(List<FieldOptions> returnFields, bool SaveFields = true, FreeForm formInstance = null)
        {
            if (formInstance == null)
            {
                formInstance = SearchFormRef;
            }
            bool changeFieldsHidden = true;
            if (returnFields == FieldsHidden)
            {
                changeFieldsHidden = false;
            }
            if (formInstance != null)
            {
                returnFields.ForEach(x =>
                {
                    if (changeFieldsHidden)
                    {
                        var fieldH = FieldsHidden.FirstOrDefault(y => y.Name == x.Name);
                        if(fieldH != null){
                            fieldH.Hidden = !x.Hidden;
                        }
                    }
                    formInstance.Panels.ForEach(y =>
                    {
                        y.Fields.ForEach(z =>
                        {
                            if (z.Name == x.Name)
                            {
                                z.Hidden = !x.Hidden;
                            }
                        });
                    });
                });
                formInstance.Refresh();
                if (SaveFields)
                {
                    localStorageService.SetItemAsync($"{BusinessName}.Search.HiddenFields", returnFields);
                }
            }
        }

        public void Onchange(bool value, dynamic data){
            if (value){
                SelectedObjects.Add(data);
            }else{
                SelectedObjects.Remove(data);
            }
        }
        /// <summary>
        /// Clear the selection of the list
        /// </summary>
        public void ClearSelection(){
            if(UseFlex && _flexComponentRef != null){
                _flexComponentRef.ClearSelection();
            }
        }
    }
}
