using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using Microsoft.AspNetCore.Components.Web;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Services;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Extension;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.FormManager;
using Siesa.Global.Enums;
using Siesa.SDK.Frontend.Components.FormManager.Views;

namespace Siesa.SDK.Frontend.Components.Fields
{
    public partial class SDKEntityField
    {
        [Inject] public IBackendRouterService BackendRouterService { get; set; }
        [Inject] public IServiceProvider ServiceProvider { get; set; }
        [Inject] public IFeaturePermissionService FeaturePermissionService { get; set; }
        [Inject] public SDKDialogService SDKDialogService { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }
        [Parameter] public string FieldName { get; set; }
        [Parameter] public string BusinessName { get; set; }
        [Parameter] public string RelatedBusiness { get; set; }
        [Parameter] public dynamic BaseObj { get; set; }
        [Parameter] public dynamic ParentBusinessObj { get; set; }
        [Parameter] public int MinCharsEntityField { get; set; } = 0;
        [Parameter] public Dictionary<string, string> RelatedFilters { get; set; } = new Dictionary<string, string>();
        [Parameter] public RelatedParams RelatedParams { get; set; }
        [Parameter] public Action<object> SetValue { get; set; }
        [Parameter] public Action OnChange { get; set; }
        [Parameter] public Action<List<dynamic>> OnReady { get; set; }
        [Parameter] public bool IsMultiple { get; set; } = false;
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public List<List<object>> Filters { get; set; }
        [Parameter] public bool CanSearch { get; set; } = true;
        public dynamic RelBusinessObj { get; set; }
        private string Value = "";
        private long rowidLastValue = -1;
        private List<string> Values = new List<string>() {};
        private IList<dynamic> ItemsSelected = new List<dynamic>() {};
        private Dictionary<int, object> CacheData = new Dictionary<int, object>();
        private IList<dynamic> CacheDataObjcts = new List<dynamic>();
        SDKBusinessModel relBusinessModel = null;
        private LoadResult CacheLoadResult;
        private string? LastSearchString;
        private CancellationTokenSource cancellationTokenSource;
        private int MinMillisecondsBetweenSearch = 200;
        private int RowidCulture = 1;
        private bool HasValue = false;
        public PropertyInfo BindProperty { get; set; }
        public Type typeProperty { get; set; }
        public string orderBy { get; set; } = "Rowid";
        public int FieldTemplate { get; set; } = 1;
        public Dictionary<string, object> DefaultFieldsCreate { get; set; } = new Dictionary<string, object>();

        private bool CanCreate;
        private bool CanEdit;
        private bool CanDetail;
        private string idInput = "";

        private string badgeContainerClass = "badge-container d-none";
        private string placeholder = "";
        private long lastRefresh;
        public bool HasError = false;
        public string ErrorMessage = "";

        private Dictionary<string, dynamic> BadgeByData = new Dictionary<string, dynamic>();
        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            await InitView();
            await LoadData("", null);
            if(RelatedParams != null && RelatedParams.AutoValueInUnique){
                if(CacheData != null && CacheData.Count == 1){
                    var item = CacheData.First().Value;
                    Value = item.ToString();
                    SetVal(item);
                    HasValue = true;
                }
            }
            if(OnReady != null){
                OnReady(CacheDataObjcts.ToList());
            }
            StateHasChanged();
        }

        private async Task BusinessNotFoundError()
        {
            HasError = true;
            ErrorMessage = await UtilsManager.GetResource("Custom.Generic.BusinessNotFoundInEntityField");
            ErrorMessage = string.Format(ErrorMessage, $"{RelatedBusiness}.Singular", RelatedBusiness);
        }

        protected async Task InitView()
        {
            idInput = Guid.NewGuid().ToString();
            CheckPermissions();
            
            var currentValueObj = BaseObj.GetType().GetProperty(FieldName).GetValue(BaseObj);
            if(currentValueObj != null && !IsMultiple)
            {
                if(currentValueObj.GetType().GetProperty("Rowid").GetValue(currentValueObj) != 0)
                {
                    Value = currentValueObj.ToString();
                    //SetVal(currentValueObj); //TODO: Probar todos los casos
                }
            }
            relBusinessModel = BackendRouterService.GetSDKBusinessModel(RelatedBusiness, null);

            if (relBusinessModel is null)
            {
                await BusinessNotFoundError();
                return;
            }
            
            var relBusinessType = Utilities.SearchType(relBusinessModel.Namespace + "." + relBusinessModel.Name);
            RelBusinessObj = ActivatorUtilities.CreateInstance(ServiceProvider, relBusinessType);
            if(AuthenticationService.User!=null){
                RowidCulture = AuthenticationService.User.RowidCulture;
            }
            if(RelatedParams != null){
                FieldTemplate = RelatedParams.FieldTemplate;
            }
            BindProperty = BaseObj.GetType().GetProperty(FieldName);
            typeProperty = BindProperty.PropertyType;
            if (Utilities.IsAssignableToGenericType(typeProperty, typeof(BaseMaster<,>))){
                orderBy = "Id";
            }else{
                orderBy = "Rowid";
            }
            StateHasChanged();
        }

        public override async Task SetParametersAsync(ParameterView parameters){
            if (parameters.TryGetValue<dynamic>("BaseObj", out dynamic baseObjNew)){
                if(BaseObj != null && baseObjNew != null && RelBusinessObj is not null){
                    BindProperty = BaseObj.GetType().GetProperty(FieldName);
                    dynamic baseObjNewRelated = baseObjNew.GetType().GetProperty(FieldName).GetValue(baseObjNew);
                    if(!IsMultiple){
                        var rowidNew = baseObjNewRelated != null ? baseObjNewRelated.GetType().GetProperty("Rowid").GetValue(baseObjNewRelated) : 0;
                        if(baseObjNewRelated != null && rowidNew != rowidLastValue){
                            CacheLoadResult = null;
                            LastSearchString = null;
                            Value = "";
                            ItemsSelected.Clear();
                            CacheData.Clear();
                            CacheDataObjcts.Clear();
                            HasValue = false;
                            SetVal(BaseObj.GetType().GetProperty(FieldName).GetValue(BaseObj));
                        }
                        BaseObj = baseObjNew;
                        if(BaseObj !=null && BaseObj.GetType().GetProperty("Rowid") != null && BaseObj.Rowid != 0){
                            Value = string.IsNullOrEmpty(BaseObj.GetType().GetProperty(FieldName).GetValue(BaseObj)?.ToString()) ? "" : BaseObj.GetType().GetProperty(FieldName).GetValue(BaseObj)?.ToString();
                        }
                        RelBusinessObj.GetType().GetProperty("BaseObj").SetValue(RelBusinessObj, baseObjNewRelated);
                        rowidLastValue = rowidNew;
                    }else{
                        if(baseObjNewRelated == null || baseObjNewRelated.Count == 0){
                            CacheLoadResult = null;
                            LastSearchString = null;
                            Value = "";
                            ItemsSelected.Clear();
                            CacheData.Clear();
                            CacheDataObjcts.Clear();
                            HasValue = false;
                            SetVal(null);
                        }
                    }
                }
            }
            
            await base.SetParametersAsync(parameters);
        }

        protected override async Task OnParametersSetAsync(){
            if(BaseObj != null && !string.IsNullOrEmpty(Value)){
                HasValue = true;
            }
            await base.OnParametersSetAsync().ConfigureAwait(true);
        }

        private async Task OnSelectItem(dynamic item){
            SetVal(item);
            if(OnChange != null){
                OnChange();
            }
            LoadData("", null, true);
            StateHasChanged();
        }

        private async Task SetVal(dynamic item, bool existItem = false)
        {
            if(item == null){
                if(SetValue != null){
                    SetValue(item);
                }else{
                    BindProperty.SetValue(BaseObj, item);
                }
                if(IsMultiple){
                    BadgeByData.Clear();
                    Values.Clear();
                    placeholder = "";
                }
                return;
            }
            if(SetValue != null){
                SetValue(item);
                if(item.Rowid != 0){
                    Value = item.ToString();
                }
            }else{
                if(item.GetType().ToString().Equals("Newtonsoft.Json.Linq.JObject")){
                    BadgeByData.Add("_item_replace", item);
                    var rowidItem = Int64.Parse(item.rowid.ToString());
                    var response = await relBusinessModel.Get(rowidItem);
                    if(response!=null){
                        if(IsMultiple){
                            item = JsonConvert.DeserializeObject(response.ToString(), typeProperty.GetGenericArguments().First());
                        }else{
                            item = JsonConvert.DeserializeObject(response.ToString(), typeProperty);
                        }
                    }
                }
                if(IsMultiple){
                    var list = BaseObj.GetType().GetProperty(FieldName).GetValue(BaseObj);
                    if(list == null){
                        list = Activator.CreateInstance(typeProperty);
                    }
                    var addMethod = typeProperty.GetMethod("Add");
                    addMethod.Invoke(list, new object[] { item });
                    BindProperty.SetValue(BaseObj, list);
                }else{
                    BindProperty.SetValue(BaseObj, item);
                }
                if(item.Rowid != 0){
                    Value = item.ToString();
                }
            }
            if(IsMultiple){
                if(BadgeByData.TryGetValue("_item_replace", out dynamic itemReplace)){
                    BadgeByData.Remove("_item_replace");
                    BadgeByData.Add(Value, itemReplace);
                }
                Values.Add(Value);
                if(!existItem){
                    ItemsSelected.Add(item);
                }
                Value = "";
                if(ItemsSelected.Count==1){
                    var tag = await UtilsManager.GetResource("Custom.EntityField.MultiSelect.Placeholder.Singular");
                    placeholder = ItemsSelected.Count + " " + tag;
                }else{
                    var tag = await UtilsManager.GetResource("Custom.EntityField.MultiSelect.Placeholder.Plural");
                    placeholder = ItemsSelected.Count + " " + tag;
                }

            }else{
                Values.Clear();
                ItemsSelected.Clear();
                HasValue = false;
                if(Value != ""){
                    Values.Add(Value);
                    ItemsSelected.Add(item);
                    HasValue = true;
                }
            }
            rowidLastValue = item.Rowid;
        }
        
        private async Task OnChangeValue(string value)
        {
            Value = value;
            if(string.IsNullOrEmpty(Value) && !IsMultiple){
                ItemsSelected.Clear();
                Values.Clear();
                HasValue = false;
            }
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
            cancellationTokenSource = new CancellationTokenSource();            
            await LoadData(value, cancellationTokenSource.Token);
            StateHasChanged();
        }

        private void OnFocus(){
            SDKDropDown();
            badgeContainerClass = "badge-container";
        }

        private async Task OnFocusOut(){
            await Task.Delay(200);
            badgeContainerClass = "badge-container d-none";
        }

        private async Task OnKeyDown(KeyboardEventArgs e)
        {
            if(e == null || e.Key == null){
                return;
            }
            if(!string.Equals(e.Key, "Escape", StringComparison.Ordinal) && !string.Equals(e.Key, "Enter", StringComparison.Ordinal) && !string.Equals(e.Key, "Tab", StringComparison.Ordinal)){
                SDKDropDown();
            }
            if(string.Equals(e.Key, "Enter", StringComparison.Ordinal))
            {
                if (CacheDataObjcts != null && CacheDataObjcts.Count > 0)
                {
                    SetVal(CacheDataObjcts.First());
                    if(IsMultiple){
                        await LoadData("", null, true).ConfigureAwait(true);
                    }else{
                        await BlurElement().ConfigureAwait(true);
                    }
                    if(OnChange != null){
                        OnChange();
                    }
                }
                StateHasChanged();
            }
            else
            {
                //concatenate the key pressed to the search string
                if (e.Key.Length == 1)
                {
                    await OnChangeValue(Value + e.Key).ConfigureAwait(true);
                }
                else if (e.Key.Equals("Backspace", StringComparison.Ordinal))
                {
                    if (Value.Length > 0)
                    {
                        await OnChangeValue(Value.Substring(0, Value.Length - 1)).ConfigureAwait(true);
                    }
                }
            }
        }

        private async Task BlurElement()
        {
            var elementInstance = await JsRuntime.InvokeAsync<IJSObjectReference>("$", $"#{idInput}").ConfigureAwait(true);
            await elementInstance.InvokeVoidAsync("dropdown", "hide").ConfigureAwait(true);
        }


        private string GetParamValue(string field, object item){
            var param = "";
            var property = item.GetType().GetProperty(field);
            if(property == null){
                return param;
            }
            var value = property.GetValue(item);
            if(value != null){
                param = value.ToString();
            }
            return param;
        }
        private async Task<LoadResult> LoadData(string searchText, CancellationToken? cancellationToken, bool force = false)
        {
            if (LastSearchString != searchText || force)
            {
                LastSearchString = searchText;
                CacheLoadResult = null;
            }
            else
            {
                return CacheLoadResult;
            }
            if (cancellationToken != null)
            {
                await Task.Delay(MinMillisecondsBetweenSearch, cancellationToken.Value);
            }

            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
            {
                return CacheLoadResult;
            }

            //check length of search text
            if (searchText.Length > MinCharsEntityField || CacheLoadResult == null)
            {
                var filters = await GetFilters();

                var extraFields = RelatedParams?.ExtraFields ?? new List<string>();
                try{
                    if (RelBusinessObj is null)
                    {
                        await BusinessNotFoundError();
                        return CacheLoadResult;
                    }
                    
                    var result = await RelBusinessObj.EntityFieldSearchAsync(searchText, filters, 10, orderBy, extraFields);

                    var response = new LoadResult
                    {
                        data = result.Data,
                        totalCount = result.TotalCount,
                        groupCount = result.GroupCount
                    };
                    CacheData.Clear();
                    CacheDataObjcts.Clear();
                    foreach (var item in result.Data){
                        CacheData.Add(item.Rowid, item);
                        CacheDataObjcts.Add(item);
                    }
                    response.totalCount = CacheData.Count;
                    CacheLoadResult = response;
                    return response;
                }catch(Exception ex){
                    HasError = true;
                    ErrorMessage = await UtilsManager.GetResource("Custom.General.SDKEntity.Error.GetData");
                    
                }
                return CacheLoadResult;
            }
            else
            {
                return CacheLoadResult;
            }
        }

        private async Task<string> GetFilters()
        {
            var filters = "";
            if (ShouldExcludeBaseObject()){
                filters = $"(Rowid != {BaseObj.Rowid})";
            }

            foreach (var item in RelatedFilters)
            {
                var value = item.Value;
                var key = item.Key;
                key = key.Replace("RelBaseObj.", "");
                dynamic dynamicValue;
                try
                {
                    dynamicValue = await Utils.Evaluator.EvaluateCode(value, BaseObj);
                }
                catch (Exception ex)
                {
                    dynamicValue = value;
                }
                if (dynamicValue == null)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(filters))
                {
                    filters += " && ";
                }
                switch (dynamicValue)
                {
                    case System.Boolean boolean:
                        filters += $"({key} == {dynamicValue})";
                        break;

                    case System.Int64 int64:
                    case System.Int32 int32:
                    case System.Double doubleValue:
                    case System.Decimal decimalValue:
                        filters += $"({key} == {dynamicValue})";
                        break;
                    default:
                        filters += $"({key} == null ? \"\" : {key}).Equals(\"{dynamicValue.ToString()}\")";
                        break;
                }
            }

            filters = await AddParameterFilter(filters).ConfigureAwait(true);

            if(IsMultiple && ItemsSelected != null && ItemsSelected.Count > 0){
                foreach (dynamic item in ItemsSelected)
                {
                    filters = AddItemsSelectedFilters(filters);
                }
            }

            return filters;
        }

        private bool ShouldExcludeBaseObject(){
            return BaseObj != null &&
                RelBusinessObj != null &&
                RelBusinessObj.BaseObj != null &&
                RelBusinessObj.BaseObj.GetType() == BaseObj.GetType() &&
                GetBaseObjRowId() != 0;
        }

        private int GetBaseObjRowId(){
            var baseObjRowidProperty = BaseObj.GetType().GetProperty("Rowid");
            return (int)(baseObjRowidProperty?.GetValue(BaseObj) ?? 0);
        }

        private async Task<string> AddParameterFilter(string filters){
            if (Filters?.Count > 0)
            {
                string paramFilter = await FormUtils.GenerateFilters(Filters, ParentBusinessObj);

                if (!string.IsNullOrEmpty(paramFilter) && !string.IsNullOrEmpty(filters))
                {
                    filters += " && ";
                }

                filters += paramFilter;
            }

            return filters;
        }

        private string AddItemsSelectedFilters(string filters){
            foreach (dynamic item in ItemsSelected)
            {
                if (!string.IsNullOrEmpty(filters))
                {
                    filters += " && ";
                }

                filters += $"(Rowid != {item.Rowid})";
            }

            return filters;
        }


        private async Task CheckPermissions()
        {
            if (FeaturePermissionService != null && !string.IsNullOrEmpty(RelatedBusiness))
            {
                try
                {
                    CanCreate = await FeaturePermissionService.CheckUserActionPermission(RelatedBusiness, enumSDKActions.Create, AuthenticationService);
                    CanEdit = await FeaturePermissionService.CheckUserActionPermission(RelatedBusiness, enumSDKActions.Edit, AuthenticationService);
                    CanDetail = await FeaturePermissionService.CheckUserActionPermission(RelatedBusiness, enumSDKActions.Detail, AuthenticationService);
                }
                catch (System.Exception)
                {
                }
            }

        }

        public async Task OnSave(object rowid){
            if(rowid != null){
                var response = await RelBusinessObj.GetDataAsync(null, null, "Rowid=" + rowid.ToString(), "");
                dynamic data = response.Data[0];
                SetVal(data);
                if(OnChange != null){
                    OnChange();
                }
                //await LoadData("", null, true);
            }
        }
        
        public void OnSelectedRow(IList<dynamic> items){
            if(items != null){
                if(!IsMultiple){
                    BadgeByData.Clear();
                    SetVal(items.First());
                    SDKDialogService.Close(true);
                }else{
                    ItemsSelected = items;
                    HasValue = true;
                }
                if(OnChange != null){
                    OnChange();
                }
                //LoadData("", null);
            }
        }
        public async Task SelectValues(){
            SDKDialogService.Close(true);
            if(ItemsSelected != null && ItemsSelected.Count > 0){
                var list = Activator.CreateInstance(typeProperty);
                BindProperty.SetValue(BaseObj, list);
                BadgeByData.Clear();
                foreach (var item in ItemsSelected){
                    await SetVal(item, true);
                }
            }
        }

        public async Task SDKDropDown(){
            var search = "";
            if(!IsMultiple){
                if(ItemsSelected.Count == 0 || !ItemsSelected[0].ToString().Equals(Value)){
                    search = Value;
                }else{
                    search = "";
                }
            }else{
                search = Value;
            }
            var currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if((currentTime - lastRefresh) > 1000)
            {
                lastRefresh = currentTime;
                var force = false;
                if(search.Equals("")){
                    force = true;
                }
                await LoadData(search, null, force);
            }
            await Task.Delay(100);
            var elementInstance = await JsRuntime.InvokeAsync<IJSObjectReference>("$", $"#{idInput}[aria-expanded=false]");
            await elementInstance.InvokeVoidAsync("dropdown","show");
            StateHasChanged();
        }
        public async Task closeItem(string item){
            Values.Remove(item);
            dynamic itemSelected = ItemsSelected.FirstOrDefault(x => x.ToString() == item);
            if(itemSelected == null){
                BadgeByData.TryGetValue(item, out itemSelected);
            }
            if(itemSelected != null){
                ItemsSelected.Remove(itemSelected);
                if(ItemsSelected.Count>0){
                    if(ItemsSelected.Count==1){
                        var tag = await UtilsManager.GetResource("Custom.EntityField.MultiSelect.Placeholder.Singular");
                        placeholder = ItemsSelected.Count + " " + tag;
                    }else{
                        var tag = await UtilsManager.GetResource("Custom.EntityField.MultiSelect.Placeholder.Plural");
                        placeholder = ItemsSelected.Count + " " + tag;
                    }
                }else{
                    placeholder = "";
                }
            }
            if(OnChange != null){
                OnChange();
            }
            StateHasChanged();
        }

        public IList<dynamic> GetItemsSelected(){
            return ItemsSelected;
        }

        protected override string GetAutomationId()
        {
            if(string.IsNullOrEmpty(AutomationId))
            {
                AutomationId = FieldName;
            }
            return base.GetAutomationId();
        }

        public async Task InheritCompany()
        {
            Type typeParent = BaseObj.GetType();
            
            if (Utilities.IsAssignableToGenericType(typeParent, typeof(BaseCompany<>)))
            {
                Type RelatedType = BindProperty.PropertyType;

                if (Utilities.IsAssignableToGenericType(RelatedType, typeof(BaseCompany<>)))
                {
                    if (BaseObj.Company != null)
                    {
                        DefaultFieldsCreate.Add("BaseObj.Company", BaseObj.Company);
                        
                         if (BaseObj.RowidCompany != null)
                         {
                              DefaultFieldsCreate.Add("BaseObj.RowidCompany", BaseObj.RowidCompany);
                         }
                    }
                }
            }

        }

        public async Task Clean(){
            ItemsSelected.Clear();
            Values.Clear();
            Value = "";
            HasValue = false;
            SetVal(null);
            if(OnChange != null){
                OnChange();
            }
            StateHasChanged();
        }
    }
}