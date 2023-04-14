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

        [Inject]
        public ILocalStorageService localStorageService { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get;set; }
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
        public String ErrorMsg = "";
        private IList<dynamic> SelectedObjects { get; set; } = new List<dynamic>();
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

        private IEnumerable<object> data;

        private bool HasCustomActions { get; set; } = false;
        private List<string> CustomActionIcons { get; set; } = new List<string>();
        private List<Button> CustomActions { get; set; }
        private string WithActions {get; set;} = "120px";
        int count;
        private bool HasExtraButtons { get; set; } = false;
        private List<Button> ExtraButtons { get; set; }
        public RadzenDataGrid<object> _gridRef;

        public List<FieldOptions> FieldsHidden { get; set; } = new List<FieldOptions>();

        public List<FieldOptions> SavedHiddenFields { get; set; } = new List<FieldOptions>();

        public string BLEntityName { get; set; }

        public string LastFilter { get; set; }
        public bool HasSearchViewdef { get; set; }

        public string FinalViewdefName { get; set; }

        private bool CanCreate;
        private bool CanEdit;
        private bool CanDelete;
        private bool CanDetail;
        private bool CanList;
        private string defaultStyleSearchForm = "search_back position-relative";
        private string StyleSearchForm { get; set; } = "search_back position-relative";
        private Radzen.DataGridSelectionMode SelectionMode { get; set; } = Radzen.DataGridSelectionMode.Single;
        Guid needUpdate;
        private string _base_filter = "";

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
            await CheckPermissions();
            var metadata = GetViewdef(bName);
            if (metadata == null || metadata == "")
            {
                //ErrorMsg = "No hay definición para la vista de lista";
                ErrorMsg = "Custom.Generic.ViewdefNotFound";
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
                    catch (System.Exception)
                    {
                        ShowList = true;
                        FieldsHidden = new List<FieldOptions>();
                    }

                    try
                    {
                        SavedHiddenFields = await localStorageService.GetItemAsync<List<FieldOptions>>($"{BusinessName}.Search.HiddenFields");
                    }
                    catch (System.Exception)
                    {
                    }

                }
                ListViewModel = JsonConvert.DeserializeObject<ListViewModel>(metadata);
                if(ListViewModel.Buttons != null && ListViewModel.Buttons.Count > 0){
                    var showButton = false;
                    ExtraButtons = new List<Button>();
                    foreach (var button in ListViewModel.Buttons){
                        if(button.ListPermission != null && button.ListPermission.Count > 0){
                            showButton = CheckPermissionsButton(button.ListPermission);
                            if(showButton){
                                ExtraButtons.Add(button);
                            }
                        }else{
                            ExtraButtons.Add(button);
                        }
                    }
                    if(ExtraButtons.Count > 0){
                        HasExtraButtons = true;
                    }
                }
                UseFlex = ListViewModel.UseFlex;
                FlexTake = ListViewModel.FlexTake;
                ShowLinkTo = ListViewModel.ShowLinkTo;
                ServerPaginationFlex = ListViewModel.ServerPaginationFlex;
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
                        CustomActionIcons = CustomActions.Select(x => x.IconClass).ToList();
                        HasCustomActions = true;
                    }
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
            StateHasChanged();

        }

        // private async Task<string> GenerateFilters(List<List<object>> filters)
        // {
        //     var filter = "";
        //     List<string> filtersOr = new List<string>();
        //     foreach (var itemAnd in filters){
        //         List<object> filtersInside = (List<object>)itemAnd;
        //         if(filtersInside.Count > 0){
        //             string filtersOrStr = "";
        //             List<string> filtersOrInside = new List<string>();
        //             foreach (var item in filtersInside){
        //                 string filtersOrInsideStr = "";
        //                 var properties = JsonConvert.DeserializeObject<dynamic>(item.ToString()).Properties();
        //                 List<string> filtersAnd = new List<string>();
        //                 foreach (var property in properties){
        //                     string filtersAndStr = "";
        //                     string name = property.Name;
        //                     var codeValue = property.Value.ToString();
        //                     dynamic dynamicValue;
        //                     if (String.IsNullOrEmpty(codeValue)){
        //                         continue;
        //                     }
        //                     dynamicValue = await Evaluator.EvaluateCode(codeValue, BusinessObj);
        //                     dynamicValue = GetFilterValue(dynamicValue);
        //                     filtersAndStr = GetFiltersStr(name, dynamicValue);
        //                     filtersAnd.Add(filtersAndStr);
        //                 }
        //                 if(filtersAnd.Count > 1){
        //                     filtersOrInsideStr += "(";
        //                     filtersOrInsideStr += string.Join(" and ", filtersAnd);
        //                     filtersOrInsideStr += ")";
        //                 }else{
        //                     filtersOrInsideStr += string.Join(" and ", filtersAnd);
        //                 }
        //                 filtersOrInside.Add(filtersOrInsideStr);
        //             }
        //             if(filtersOrInside.Count > 1){
        //                 filtersOrStr += "(";
        //                 filtersOrStr += string.Join(" or ", filtersOrInside);
        //                 filtersOrStr += ")";
        //             }else{
        //                 filtersOrStr += string.Join(" or ", filtersOrInside);
        //             }
        //             filtersOr.Add(filtersOrStr);
        //         }
        //     }
        //     if(filtersOr.Count > 1){
        //         filter += "(";
        //         filter += string.Join(" and ", filtersOr);
        //         filter += ")";
        //     }else{
        //         filter += string.Join(" and ", filtersOr);
        //     }
        //     return filter;
        // }

        // private string GetFiltersStr(string name, dynamic dynamicValue)
        // {
        //     string filtersAndStr = "(";
        //     Type type = dynamicValue.GetType();
        //     bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) ? true : false;
        //     if(name.EndsWith("__in")){
        //         var list = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(dynamicValue));
        //         if(list.Count > 0){
        //             name = name.Replace("__in", "");
        //             foreach (var itemIn in list){
        //                 dynamic itemInValue = GetFilterValue(itemIn);
        //                 Type typeIn = itemIn.GetType();
        //                 if(filtersAndStr != "("){
        //                     filtersAndStr += " or ";
        //                 }
        //                 if(typeIn == typeof(int?) || typeIn == typeof(int) || typeIn == typeof(decimal?) || typeIn == typeof(decimal) || typeIn == typeof(byte?) || typeIn == typeof(byte)){
        //                     filtersAndStr += $"({name} == null ? 0 : {name}) == {itemInValue}";
        //                 }else if(typeIn == typeof(bool) || typeIn == typeof(bool?)){
        //                     filtersAndStr += $"({name} == null ? false : {name}) == {itemInValue}";
        //                 }else if(typeIn == typeof(string)){
        //                     filtersAndStr += $"({name} == null ? \"\" : {name}) == {itemInValue}";
        //                 }
        //                 else{
        //                     filtersAndStr += $"{name} == {itemInValue}";
        //                 }
        //             }
        //         }
        //     }else if(name.EndsWith("__notin")){
        //         var list = (List<object>)dynamicValue;
        //         if(list.Count > 0){
        //             name = name.Replace("__notin", "");
        //             foreach (var itemIn in list){
        //                 dynamic itemInValue = GetFilterValue(itemIn);
        //                 Type typeIn = itemIn.GetType();
        //                 if(filtersAndStr != "("){
        //                     filtersAndStr += " and ";
        //                 }
        //                 if(typeIn == typeof(int?) || typeIn == typeof(int) || typeIn == typeof(decimal?) || typeIn == typeof(decimal) || typeIn == typeof(byte?) || typeIn == typeof(byte)){
        //                     filtersAndStr += $"({name} == null ? 0 : {name}) != {itemInValue}";
        //                 }else if(typeIn == typeof(bool) || typeIn == typeof(bool?)){
        //                     filtersAndStr += $"({name} == null ? false : {name}) != {itemInValue}";
        //                 }else if(typeIn == typeof(string)){
        //                     filtersAndStr += $"({name} == null ? \"\" : {name}) != {itemInValue}";
        //                 }else{
        //                     filtersAndStr += $"{name} != {itemInValue}";
        //                 }
        //             }
        //         }
        //     }
        //     else if(name.EndsWith("__gt")){
        //         name = name.Replace("__gt", "");
        //         if(type == typeof(int?) || type == typeof(int) || type == typeof(decimal?) || type == typeof(decimal) || type == typeof(byte?) || type == typeof(byte)){
        //             filtersAndStr += $"({name} == null ? 0 : {name}) > {dynamicValue}";
        //         }else{
        //             filtersAndStr += $"{name} > {dynamicValue}";
        //         }
        //     }else if(name.EndsWith("__gte")){
        //         name = name.Replace("__gte", "");
        //         if(type == typeof(int?) || type == typeof(int) || type == typeof(decimal?) || type == typeof(decimal) || type == typeof(byte?) || type == typeof(byte)){
        //             filtersAndStr += $"({name} == null ? 0 : {name}) >= {dynamicValue}";
        //         }else{
        //             filtersAndStr += $"{name} >= {dynamicValue}";
        //         }                
        //     }else if(name.EndsWith("__lt")){
        //         name = name.Replace("__lt", "");
        //         if(type == typeof(int?) || type == typeof(int) || type == typeof(decimal?) || type == typeof(decimal) || type == typeof(byte?) || type == typeof(byte)){
        //             filtersAndStr += $"({name} == null ? 0 : {name}) < {dynamicValue}";
        //         }else{
        //             filtersAndStr += $"{name} < {dynamicValue}";
        //         }
        //     }else if(name.EndsWith("__lte")){
        //         name = name.Replace("__lte", "");
        //         if(type == typeof(int?) || type == typeof(int) || type == typeof(decimal?) || type == typeof(decimal) || type == typeof(byte?) || type == typeof(byte)){
        //             filtersAndStr += $"({name} == null ? 0 : {name}) <= {dynamicValue}";
        //         }else{
        //             filtersAndStr += $"{name} <= {dynamicValue}";
        //         }
        //     }else if(name.EndsWith("__contains")){
        //         name = name.Replace("__contains", "");
        //         filtersAndStr += $"({name} == null ? \"\" : {name}).ToLower().Contains(\"{dynamicValue}\".ToLower())";
        //     }else{
        //         if(type == typeof(int?) || type == typeof(int) || type == typeof(decimal?) || type == typeof(decimal) || type == typeof(byte?) || type == typeof(byte)){
        //             filtersAndStr += $"({name} == null ? 0 : {name}) == {dynamicValue}";
        //         }else if(type == typeof(bool) || type == typeof(bool?)){
        //             filtersAndStr += $"({name} == null ? false : {name}) == {dynamicValue}";
        //         }else if(type == typeof(string)){
        //             filtersAndStr += $"({name} == null ? \"\" : {name}).ToLower() == \"{dynamicValue}\".ToLower()";
        //         }else{
        //             filtersAndStr += $"{name} == {dynamicValue}";
        //         }
        //     }
        //     filtersAndStr += ")";
        //     return filtersAndStr;
        // }

        // private dynamic GetFilterValue(dynamic dynamicValue)
        // {
        //     Type type = dynamicValue.GetType();
        //     dynamic filterValue = dynamicValue;
        //     if(type.IsEnum){
        //         filterValue = (int)dynamicValue;
        //     }
        //     return filterValue;
        // }

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
                }catch(System.Exception){

                }
            }
            return showButton;
        }

        private async Task CheckPermissions()
        {
            if (FeaturePermissionService != null && !string.IsNullOrEmpty(BusinessName))
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

                if (!CanList)
                {
                    ErrorMsg = "Custom.Generic.Unauthorized";
                    NotificationService.ShowError("Custom.Generic.Unauthorized");
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
        /*protected override void OnAfterRender(bool firstRender){
            if(SelectedItems!=null && firstRender){
                foreach (var item in SelectedItems){
                    SelectedObjects.Add(item);
                }
            }
            base.OnAfterRender(firstRender);
        }*/
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
                        if (parameters.TryGetValue<string>(property.Name, out var value)){
                            var valueProperty = property.GetValue(this, null);
                            if (value != null && value != valueProperty){
                                result = true;
                                break;
                            }
                        }
                    }catch (Exception e){}
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
            guidListView = Guid.NewGuid().ToString();
            Loading = false;
            ErrorMsg = "";
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
                if (BusinessObj.BaseObj.RowidCompany != 0)
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
            catch (System.Exception)
            {

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

                        var fieldObj = field.GetFieldObj(BusinessObjNullable);
                        if (fieldObj != null)
                        {
                            dynamic searchValue = fieldObj.ModelObj.GetType().GetProperty(fieldObj.Name).GetValue(fieldObj.ModelObj, null);
                            if (searchValue == null)
                            {
                                continue;
                            }
                            //check if searchValue is an empty string
                            if (searchValue is string && string.IsNullOrEmpty(searchValue))
                            {
                                continue;
                            }
                            /*try{
                                Type searchValueType = searchValue.GetType();
                                dynamic defaultValue = null;
                                if(searchValueType.IsValueType && !searchValueType.IsEnum){
                                    defaultValue = Activator.CreateInstance(searchValueType);
                                }
                                if (searchValue == defaultValue)
                                {
                                    continue;
                                }
                            }catch(Exception e){
                                Console.WriteLine(e);
                            }*/
                            switch (fieldObj.FieldType)
                            {
                                case FieldTypes.CharField:
                                case FieldTypes.TextField:
                                    tmpFilter = $"({fieldObj.Name} == null ? \"\" : {fieldObj.Name}).ToLower().Contains(\"{searchValue}\".ToLower())";
                                    break;
                                case FieldTypes.IntegerField:
                                case FieldTypes.DecimalField:
                                case FieldTypes.SmallIntegerField:
                                case FieldTypes.BigIntegerField:
                                case FieldTypes.ByteField:
                                    if (!fieldObj.IsNullable)
                                    {
                                        tmpFilter = $"{fieldObj.Name} == {searchValue}";
                                    }
                                    else
                                    {
                                        tmpFilter = $"({fieldObj.Name} == null ? 0 : {fieldObj.Name}) == {searchValue}";
                                    }
                                    break;
                                case FieldTypes.BooleanField:
                                    if (!searchValue)
                                    {
                                        break;
                                    }

                                    if (!fieldObj.IsNullable)
                                    {
                                        tmpFilter = $"{fieldObj.Name} == {searchValue}";
                                    }
                                    else
                                    {
                                        tmpFilter = $"({fieldObj.Name} == null ? false : {fieldObj.Name}) == {searchValue}";
                                    }
                                    break;
                                case FieldTypes.DateField:
                                case FieldTypes.DateTimeField:
                                    if (!fieldObj.IsNullable)
                                    {
                                        tmpFilter = $"{fieldObj.Name} == DateTime.Parse(\"{searchValue}\")";
                                    }
                                    else
                                    {
                                        tmpFilter = $"({fieldObj.Name} == null ? DateTime.MinValue : {fieldObj.Name}) == DateTime.Parse(\"{searchValue}\")";
                                    }
                                    break;

                                case FieldTypes.EntityField:
                                    if (!fieldObj.IsNullable)
                                    {
                                        tmpFilter = $"Rowid{fieldObj.Name} == {searchValue.Rowid}";
                                    }
                                    else
                                    {
                                        tmpFilter = $"({fieldObj.Name} == null ? 0 : {fieldObj.Name}.Rowid) == {searchValue.Rowid}";
                                    }
                                    break;
                                
                                case FieldTypes.Custom:
                                case FieldTypes.SelectField:
                                    if (field.CustomType == "SelectBarField" || field.FieldType == FieldTypes.SelectField)
                                    {
                                        Type enumType = searchValue.GetType();
                                        var EnumValues = Enum.GetValues(enumType);
                                        var LastValue = EnumValues.GetValue(EnumValues.Length - 1);
                                        
                                        if (Convert.ToInt32(LastValue)+1 != Convert.ToInt32(searchValue))
                                        { 
                                            tmpFilter = $"{fieldObj.Name} == {Convert.ToInt32(searchValue)}";
                                        }
                                    }

                                    break;
                                default:
                                    break;
                            }
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
            catch (System.Exception)
            {
            }

            return filters;
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
            if(!UseFlex && !ListViewModel.InfiniteScroll){
                includeCount = true;
            }
            var dbData = await BusinessObj.GetDataAsync(args.Skip, args.Top, filters, args.OrderBy, includeCount);
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
                NavManager.NavigateTo($"{BusinessName}/edit/{id}/");
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

        private void GoToDetail(Int64 id)
        {
            if (OnClickDetail != null)
            {
                OnClickDetail(id.ToString());
            }
            else
            {
                NavManager.NavigateTo($"{BusinessName}/detail/{id}/");
            }
        }

        [JSInvokable]
        public async Task<bool> DeleteFromReact(Int64 id, string object_string)
        {
            if (OnClickDelete != null){
                OnClickDelete(id.ToString(), object_string);
            }
            if (UseFlex)
            {
                var confirm = await ConfirmDelete();
                SDKGlobalLoaderService.Show();
                if (confirm){
                    BusinessObj.BaseObj.Rowid = Convert.ChangeType(id, BusinessObj.BaseObj.Rowid.GetType());
                    var result = await BusinessObj.DeleteAsync();
                    SDKGlobalLoaderService.Hide();
                    if (result != null && result.Errors.Count == 0){
                        return true;
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
            IList<object> objects = JsonConvert.DeserializeObject<IList<object>>(item);
            OnSelectionChanged(objects);
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
                 if (Data != null && Data.Count() == 1){
                    GoToDetail((dynamic)Data.First());
                     return;
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
                if(!UseFlex && !ListViewModel.InfiniteScroll){
                    includeCount = true;
                    skip = 0;
                    take = ListViewModel.Paging.PageSize;
                    if(_gridRef != null){
                        var currentPage = _gridRef.GetType().GetProperty("CurrentPage").GetValue(_gridRef);
                        skip = (int)currentPage * ListViewModel.Paging.PageSize;
                    }
                }
                var dbData = await BusinessObj.GetDataAsync(skip, take, filters, "", includeCount);
                count = dbData.TotalCount;
                data = dbData.Data;
                if (count == 1){
                    if(!FromEntityField){
                        GoToDetail(((dynamic)data.First()).Rowid);
                    }else{
                        IList<object> objects = new List<object> { data.First()};
                        OnSelectionChanged(objects);
                    }
                    return;
                }

                if(!UseFlex && ListViewModel.InfiniteScroll){
                    Data = data;
                }
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
            catch (System.Exception)
            {
                _ = InvokeAsync(() => StateHasChanged());
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

                var eject = await Evaluator.EvaluateCode(button.Action, BusinessObj, button.Action, true);
                if (eject != null){
                    eject(obj);
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
                            catch (Exception e)
                            {
                                throw;
                            }
                        }
                        if (CustomAttr.Key == "sdk-show")
                        {
                            try
                            {
                                code += @$"
                                try {{ ListViewFields[{i}].Hidden = !({(string)CustomAttr.Value}); }} catch (Exception ex) {{ throw;}}";
                            }
                            catch (Exception e)
                            {
                                throw;
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
    }
}
