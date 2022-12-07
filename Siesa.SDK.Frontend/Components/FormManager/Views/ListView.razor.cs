using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
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
        public bool UseFlex { get; set; } = false;
        [Parameter]
        public int FlexTake { get; set; } = 100;
        [Parameter]
        public bool ServerPaginationFlex { get; set; } = false;
        [Parameter]
        public bool ShowLinkTo {get; set;} = false;

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

        public bool Loading;
        public bool LoadingData;
        public bool LoadingSearch;

        private bool _isEditingFlex = false;
        private bool _isSearchOpen = false;
        public String ErrorMsg = "";
        private IList<object> SelectedObjects { get; set; }

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
        public Action<object> OnSelectedRow { get; set; } = null;

        [Parameter]
        public IEnumerable<object> Data { get; set; } = null;

        private IEnumerable<object> data;

        private bool HasCustomActions { get; set; } = false;
        private string WithActions {get; set;} = "100px";
        int count;
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


        Guid needUpdate;

        private void OnSelectionChanged(IList<object> objects)
        {
            if (OnSelectedRow != null)
            {
                SelectedObjects = objects;
                if (SelectedObjects?.Any() == true)
                {
                    OnSelectedRow(objects.First());
                }
                else
                {
                    OnSelectedRow(null);
                }

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
            }
            return data;
        }

        protected async void InitView(string bName = null)
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
                ErrorMsg = "No hay definición para la vista de lista";
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
                UseFlex = ListViewModel.UseFlex;
                FlexTake = ListViewModel.FlexTake;
                ShowLinkTo = ListViewModel.ShowLinkTo;
                ServerPaginationFlex = ListViewModel.ServerPaginationFlex;
                if(ListViewModel.CustomActions != null && ListViewModel.CustomActions.Count > 0){
                    HasCustomActions = true;
                    var withInt = (ListViewModel.CustomActions.Count+2)*40;
                    WithActions = $"{withInt}px";
                }
                foreach (var field in ListViewModel.Fields)
                {
                    field.GetFieldObj(BusinessObj);
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

        public async Task Refresh(bool Reload = false)
        {
            if (Reload)
            {
                Restart();
            }
            hideCustomColumn();
            StateHasChanged();
        }

        private async Task CheckPermissions()
        {
            if (FeaturePermissionService != null)
            {
                try
                {
                    CanList = await FeaturePermissionService.CheckUserActionPermission(BusinessName, 4, AuthenticationService);
                    CanCreate = await FeaturePermissionService.CheckUserActionPermission(BusinessName, 1, AuthenticationService);
                    CanEdit = await FeaturePermissionService.CheckUserActionPermission(BusinessName, 2, AuthenticationService);
                    CanDelete = await FeaturePermissionService.CheckUserActionPermission(BusinessName, 3, AuthenticationService);
                    CanDetail = await FeaturePermissionService.CheckUserActionPermission(BusinessName, 5, AuthenticationService);
                }
                catch (System.Exception)
                {
                }

                if (!CanList)
                {
                    ErrorMsg = "Unauthorized";
                    NotificationService.ShowError("Custom.Generic.Unauthorized");
                    NavigationService.NavigateTo("/", replace: true);
                }

            }

        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            guidListView = Guid.NewGuid().ToString();            
            Restart();
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

        private void Restart()
        {

            Loading = false;
            ErrorMsg = "";
            InitView();
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

                        var fieldObj = field.GetFieldObj(BusinessObj);
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
                foreach (var d in Data)
                {
                    var s = d;
                }
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

            var dbData = await BusinessObj.GetDataAsync(args.Skip, args.Top, filters, args.OrderBy);
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
                if (confirm){
                    BusinessObj.BaseObj.Rowid = Convert.ToInt32(id);
                    var result = await BusinessObj.DeleteAsync();
                    if (result != null && result.Errors.Count == 0){
                        return true;
                    }
                }
            }
            return false;
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
            var filters = GetFilters();
            if (Data == null)
            {
                Data = new List<object> { };
            }
            if(ServerPaginationFlex ){
                var dbData = await BusinessObj.GetDataAsync(0, 2, filters, "");
                Data = dbData.Data;
                if (Data.Count() == 1)
                {
                    GoToDetail(((dynamic)Data.First()).Rowid);
                    return;
                }
                count = dbData.TotalCount;
            }else{
                var dbData = await BusinessObj.GetDataAsync(null, null, filters, "");
                Data = dbData.Data;
                if (Data.Count() == 1)
                {
                    GoToDetail(((dynamic)Data.First()).Rowid);
                    return;
                }
                count = dbData.TotalCount;
                data = Data;
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
    }
}
