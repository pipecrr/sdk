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
        [Parameter] public int MinCharsEntityField { get; set; } = 2;
        [Parameter] public Dictionary<string, string> RelatedFilters { get; set; } = new Dictionary<string, string>();
        [Parameter] public RelatedParams RelatedParams { get; set; }

        [Parameter] public Action<object> SetValue { get; set; }
        [Parameter] public Action OnChange { get; set; }
        [Parameter] public bool IsMultiple { get; set; } = false;
        [Parameter] public bool Disabled { get; set; }
        public dynamic RelBusinessObj { get; set; }
        private string Value = "";
        private long rowidLastValue = 0;
        private List<string> Values = new List<string>() {};
        private IList<dynamic> ItemsSelected = new List<dynamic>() {};
        private Dictionary<int, object> CacheData = new Dictionary<int, object>();
        SDKBusinessModel relBusinessModel = null;
        private LoadResult CacheLoadResult;
        private string LastSearchString;
        private CancellationTokenSource cancellationTokenSource;
        private int MinMillisecondsBetweenSearch = 100;
        private int RowidCulture = 1;
        public PropertyInfo BindProperty { get; set; }
        public Type typeProperty { get; set; }
        public int FieldTemplate { get; set; } = 1;

        private bool CanCreate;
        private bool CanEdit;
        private bool CanDetail;
        private string idInput = "";

        private string badgeContainerClass = "badge-container d-none";
        private string placeholder = "";

        private Dictionary<string, dynamic> BadgeByData = new Dictionary<string, dynamic>();
        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            await InitView();
        }

        protected async Task InitView(){
            idInput = Guid.NewGuid().ToString();
            CheckPermissions();
            var currentValueObj = BaseObj.GetType().GetProperty(FieldName).GetValue(BaseObj);
            if(currentValueObj != null){
                Value = currentValueObj.ToString();
            }
            relBusinessModel = BackendRouterService.GetSDKBusinessModel(RelatedBusiness, null);
            var relBusinessType = Utilities.SearchType(relBusinessModel.Namespace + "." + relBusinessModel.Name);
            RelBusinessObj = ActivatorUtilities.CreateInstance(ServiceProvider, relBusinessType);
            if(AuthenticationService.User!=null){
                RowidCulture = AuthenticationService.User.RowidCulture;
            }
            if(RelatedParams != null){
                FieldTemplate = RelatedParams.FieldTemplate;
            }
            await LoadData("", null);
            BindProperty = BaseObj.GetType().GetProperty(FieldName);
            typeProperty = BindProperty.PropertyType;
            StateHasChanged();
        }

        public override async Task SetParametersAsync(ParameterView parameters){
            if (parameters.TryGetValue<dynamic>("BaseObj", out dynamic baseObjNew) && !IsMultiple){
                if(BaseObj != null && baseObjNew != null){
                    dynamic baseObjNewRelated = baseObjNew.GetType().GetProperty(FieldName).GetValue(baseObjNew);
                    if(baseObjNewRelated != null && baseObjNewRelated.Rowid != rowidLastValue){
                        SetVal(BaseObj.GetType().GetProperty(FieldName).GetValue(BaseObj));
                    }
                }
            }
            
            await base.SetParametersAsync(parameters);
        }
        
        protected override async Task OnParametersSetAsync(){
            await base.OnParametersSetAsync();
            await LoadData("", null, true);
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
                return;
            }
            if(SetValue != null){
                SetValue(item);
                Value = item.ToString();
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
                Value = item.ToString();
                if(item.Rowid == 0){
                    Value = "";
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
                if(Value != ""){
                    Values.Add(Value);
                    ItemsSelected.Add(item);
                }
                rowidLastValue = long.Parse(item.Rowid.ToString());
            }
        }
        
        private async Task OnChangeValue(string value)
        {
            Value = value;
            if(string.IsNullOrEmpty(Value) && !IsMultiple){
                ItemsSelected.Clear();
                Values.Clear();
            }
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
            cancellationTokenSource = new CancellationTokenSource();
            if(OnChange != null){
                OnChange();
            }
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

        private void OnKeyDown(KeyboardEventArgs e)
        {
            if (!e.Key.Equals("Escape"))
            {
                SDKDropDown();
            }

            if (e.Key == "Enter")
            {
                if (CacheLoadResult != null && CacheLoadResult.data != null)
                {
                    var results = CacheLoadResult.data as IEnumerable<dynamic>;

                    if (results.Count() > 0)
                    {
                        SetVal(results.First());
                        LoadData("", null,true);
                    }
                }
                StateHasChanged();
            }
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
            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
            {
                return CacheLoadResult;
            }
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

            //check length of search text
            if (searchText.Length > MinCharsEntityField || CacheLoadResult == null)
            {
                var filters = await GetFilters();
                var result = await RelBusinessObj.EntityFieldSearchAsync(searchText, filters, 3);
                var response = new LoadResult
                {
                    data = result.Data,
                    totalCount = result.TotalCount,
                    groupCount = result.GroupCount
                };
                CacheData.Clear();
                foreach (var item in result.Data){
                    CacheData.Add(item.Rowid, item);
                }
                CacheLoadResult = response;
                return response;
            }
            else
            {
                return CacheLoadResult;
            }
        }

        private async Task<string> GetFilters()
        {
            var filters = "";
            if (BaseObj != null && RelBusinessObj != null && RelBusinessObj.BaseObj != null && RelBusinessObj.BaseObj.GetType() == BaseObj.GetType())
            {
                var baseObjRowidProperty = BaseObj.GetType().GetProperty("Rowid");
                if(baseObjRowidProperty != null && baseObjRowidProperty.GetValue(BaseObj) != null &&baseObjRowidProperty.GetValue(BaseObj) != 0 )
                {
                    filters = $"(Rowid != {BaseObj.Rowid})";
                }
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
            return filters;
        }

        private async Task CheckPermissions()
        {
            if (FeaturePermissionService != null && !string.IsNullOrEmpty(RelatedBusiness))
            {
                try
                {
                    CanCreate = FeaturePermissionService.CheckUserActionPermission(RelatedBusiness, 1, AuthenticationService);
                    CanEdit = FeaturePermissionService.CheckUserActionPermission(RelatedBusiness, 2, AuthenticationService);
                    CanDetail = FeaturePermissionService.CheckUserActionPermission(RelatedBusiness, 5, AuthenticationService);
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
                await LoadData("", null, true);
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
                }
                LoadData("", null);
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
            //wait 200ms
            await Task.Delay(200);
            var elementInstance = await JsRuntime.InvokeAsync<IJSObjectReference>("$", $"#{idInput}[aria-expanded=false]");
            await elementInstance.InvokeVoidAsync("dropdown","show");
        }

        public async Task<string> GetStringFilters(){
            var filters = await GetFilters();
            var filtersSearch = "";
            if(Value != null && Value != "" && ItemsSelected.Count == 0){
                var properties = RelBusinessObj.BaseObj.GetType().GetProperties();
                foreach (var property in properties){
                    if(property.PropertyType == typeof(string)){
                        if(!string.IsNullOrEmpty(filtersSearch)){
                            filtersSearch += " || ";
                        }
                        filtersSearch += $"({property.Name} == null ? \"\" : {property.Name}).ToLower().Contains(\"{Value}\".ToLower())";
                    }
                }
            }
            if(!string.IsNullOrEmpty(filtersSearch)){
                if(!string.IsNullOrEmpty(filters)){
                    filters += " && ";
                }
                filters += $"({filtersSearch})";
            }
            return filters;
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
    }
}