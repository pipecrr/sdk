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

namespace Siesa.SDK.Frontend.Components.Fields
{
    public partial class SDKEntityField
    {
        [Inject] public IAuthenticationService AuthenticationService { get; set; }
        [Inject] public IBackendRouterService BackendRouterService { get; set; }
        [Inject] public IServiceProvider ServiceProvider { get; set; }
        [Inject] public IFeaturePermissionService FeaturePermissionService { get; set; }
        [Inject] public SDKDialogService SDKDialogService { get; set; }
        [Inject] public IJSRuntime jsRuntime { get; set; }

        [Parameter] public string FieldName { get; set; }
        [Parameter] public string BusinessName { get; set; }
        [Parameter] public string RelatedBusiness { get; set; }
        [Parameter] public dynamic BaseObj { get; set; }    
        [Parameter] public int MinCharsEntityField { get; set; } = 2;
        [Parameter] public Dictionary<string, string> RelatedFilters { get; set; } = new Dictionary<string, string>();
        [Parameter] public RelatedParams RelatedParams { get; set; }

        [Parameter] public Action<object> SetValue { get; set; }
        [Parameter] public Action OnChange { get; set; }
        public dynamic RelBusinessObj { get; set; }
        private string Value = "";
        private Dictionary<int, object> CacheData = new Dictionary<int, object>();
        SDKBusinessModel relBusinessModel = null;
        private LoadResult CacheLoadResult;
        private string LastSearchString;    
        private CancellationTokenSource cancellationTokenSource;
        private int MinMillisecondsBetweenSearch = 100;
        private int RowidCulture = 1;
        public PropertyInfo BindProperty { get; set; }
        public int FieldTemplate { get; set; } = 1;

        private bool CanCreate;
        private bool CanEdit;
        private bool CanDetail;
        private string idInput = "";

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            await InitView();
            
        }

        protected async Task InitView(){
            idInput = Guid.NewGuid().ToString();
            CheckPermissions();
            SetVal(BaseObj.GetType().GetProperty(FieldName).GetValue(BaseObj));
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
            StateHasChanged();
        }

        protected override async Task OnParametersSetAsync()
        {
            base.OnParametersSetAsync();
            await InitView();
        }

        private async Task OnSelectItem(dynamic item){
            SetVal(item);
            LoadData("", null, true);
            if(OnChange != null){
                OnChange();
            }
            StateHasChanged();
        }

        private void SetVal(dynamic item)
        {
            if(item == null){
                return;
            }
            if(SetValue != null){
                SetValue(item);
                Value = item.ToString();
            }else{
                BindProperty = BaseObj.GetType().GetProperty(FieldName);
                BindProperty.SetValue(BaseObj, item);
                Value = item.ToString();
            }
        }
        
        private async Task OnChangeValue(string value)
        {
            Value = value;
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
            cancellationTokenSource = new CancellationTokenSource();
            await LoadData(value, cancellationTokenSource.Token);            
            StateHasChanged();
        }

        private void OnFocus()
        {
            SDKDropDown();
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
                var filters = "";
                if (BaseObj != null && RelBusinessObj != null && RelBusinessObj.BaseObj != null && RelBusinessObj.BaseObj.GetType() == BaseObj.GetType())
                {
                    var baseObjRowidProperty = BaseObj.GetType().GetProperty("Rowid");
                    if(baseObjRowidProperty != null && baseObjRowidProperty.GetValue(BaseObj, 0) != 0 )
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
                var result = await RelBusinessObj.EntityFieldSearchAsync(searchText, filters);
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

        private async Task CheckPermissions()
        {
            if (FeaturePermissionService != null)
            {
                try
                {
                    CanCreate = await FeaturePermissionService.CheckUserActionPermission(RelatedBusiness, 1, AuthenticationService);
                    CanEdit = await FeaturePermissionService.CheckUserActionPermission(RelatedBusiness, 2, AuthenticationService);
                    CanDetail = await FeaturePermissionService.CheckUserActionPermission(RelatedBusiness, 5, AuthenticationService);
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
        
        public void OnSelectedRow(object item){
            if(item != null){
                SetVal(item);
                LoadData("", null);
                SDKDialogService.Close(true);
            }
        }

        public async Task SDKDropDown(){
            //wait 200ms
            await Task.Delay(200);
            var elementInstance = await jsRuntime.InvokeAsync<IJSObjectReference>("$", $"#{idInput}[aria-expanded=false]");
            await elementInstance.InvokeVoidAsync("dropdown","show");
        }
    }
}