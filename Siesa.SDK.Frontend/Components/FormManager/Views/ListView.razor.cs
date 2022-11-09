﻿using System;
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

        private FreeForm SearchFormRef;

        public string SearchFormID = Guid.NewGuid().ToString();

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }
        
        [Inject] public SDKNotificationService NotificationService { get; set; }

        [Inject] public NavigationService NavigationService { get; set; }
        [Inject] public IFeaturePermissionService FeaturePermissionService { get; set; }
        [Inject] public IAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public IBackendRouterService BackendRouterService { get; set; }

        public bool Loading;
        public bool LoadingData;
        public bool LoadingSearch;

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
        int count;
        public RadzenDataGrid<object> _gridRef;

        public string BLEntityName { get; set; }

        public string LastFilter { get; set; }
        public bool HasSearchViewdef { get; set; }

        private bool CanCreate;
        private bool CanEdit;
        private bool CanDelete;
        private bool CanDetail;
        private bool CanList;
        private string StyleSearchForm { get; set; } = "search_back position-relative mb-3";


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
                if(ShowSearchForm)
                {
                    try
                    {
                        var searchMetadata = BackendRouterService.GetViewdef(bName, "search");
                        HasSearchViewdef = !String.IsNullOrEmpty(searchMetadata);
                        ShowList = !HasSearchViewdef;
                    }
                    catch (System.Exception)
                    {
                        ShowList = true;
                    }

                }
                ListViewModel = JsonConvert.DeserializeObject<ListViewModel>(metadata);
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

                if(!CanList){
                    ErrorMsg = "Unauthorized";
                    NotificationService.ShowError("Custom.Generic.Unauthorized");
                    NavigationService.NavigateTo("/", replace:true);
                }

            }

        }

        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
            //await CheckPermissions();
            //InitView();
        }

        protected override void OnParametersSet()
        {
            Restart();
        }

        private void Restart(){

            Loading = false;
            ErrorMsg = "";
            InitView();
            data = null;
            if(Data != null){
                data = Data;
                count = data.Count();
            }
            if (_gridRef != null)
            {
                needUpdate = Guid.NewGuid();
                _gridRef.Reload();
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
            try{
                Type type = BusinessObj.BaseObj.GetType();
                if(BusinessObj.BaseObj.RowidCompany != 0)
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
            }catch(System.Exception)
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
                if(SearchFormRef != null){
                    var searchFields = SearchFormRef.GetFields();
                    foreach (var field in searchFields)
                    {
                        var tmpFilter = "";
                        
                        var fieldObj = field.GetFieldObj(BusinessObj);
                        if (fieldObj != null)
                        {
                            dynamic searchValue = fieldObj.ModelObj.GetType().GetProperty(fieldObj.Name).GetValue(fieldObj.ModelObj, null);
                            if(searchValue == null){
                                continue;
                            }
                            //check if searchValue is an empty string
                            if(searchValue is string && string.IsNullOrEmpty(searchValue)){
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
                                    if(!fieldObj.IsNullable)
                                    {
                                        tmpFilter = $"{fieldObj.Name} == {searchValue}";
                                    }
                                    else
                                    {
                                        tmpFilter = $"({fieldObj.Name} == null ? 0 : {fieldObj.Name}) == {searchValue}";
                                    }
                                    break;
                                case FieldTypes.BooleanField:
                                    if(!searchValue)
                                    {
                                        break;
                                    }

                                    if(!fieldObj.IsNullable)
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
                                    if(!fieldObj.IsNullable)
                                    {
                                        tmpFilter = $"{fieldObj.Name} == DateTime.Parse(\"{searchValue}\")";
                                    }
                                    else
                                    {
                                        tmpFilter = $"({fieldObj.Name} == null ? DateTime.MinValue : {fieldObj.Name}) == DateTime.Parse(\"{searchValue}\")";
                                    }
                                    break;

                                case FieldTypes.EntityField:
                                    if(!fieldObj.IsNullable)
                                    {
                                        tmpFilter = $"Rowid{fieldObj.Name} == {searchValue.Rowid}";
                                    }else{
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
            if(Data != null){
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

        private void GoToDelete(Int64 id, string object_string)
        {
            if (OnClickDelete != null)
            {
                OnClickDelete(id.ToString(), object_string);
            }
        }

        private async Task OnClickSearch()
        {
            LoadingSearch = true;
            LoadingData = true;
            data = null;
            var filters = GetFilters();
            if(Data == null)
            {
                Data = new List<object> { };
            }
            var dbData = await BusinessObj.GetDataAsync(null, null, filters, "");
            Data = dbData.Data;
            if(Data.Count() == 1)
            {
                GoToDetail(((dynamic)Data.First()).Rowid);
                return;
            }
            data = Data;
            count = dbData.TotalCount;
            LoadingData = false;
            LoadingSearch = false;
            ShowList = true;
            SetSearchFromVisibility(true);
        }

        public void SetSearchFromVisibility(bool hideForm){
            if(hideForm){
                StyleSearchForm = "search_back search_back_hide position-relative";
            }
            else
            {
                StyleSearchForm = "search_back position-relative";
            }
            StateHasChanged();
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

        private IDictionary<string, object> GetSelectFieldParameters(object data, FieldOptions field, string fieldName)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("BindModel", data);
            parameters.Add("FieldName", fieldName);
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
    }
}
