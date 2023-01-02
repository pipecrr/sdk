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

namespace Siesa.SDK.Frontend.Components.Fields
{
    public partial class SDKEntityField
    {
        [Inject] IAuthenticationService AuthenticationService { get; set; }
        [Inject] IBackendRouterService BackendRouterService { get; set; }
        [Inject] IServiceProvider ServiceProvider { get; set; }
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
        private LoadResult CacheLoadResult;
        private string LastSearchString;    
        private CancellationTokenSource cancellationTokenSource;
        private int MinMillisecondsBetweenSearch = 100;
        private string classList = "whcm_select_list d-none";
        private bool isFocused = false;
        private int RowidCulture = 1;
        public PropertyInfo BindProperty { get; set; }
        public int FieldTemplate { get; set; } = 1;
        
        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            SetVal(BaseObj.GetType().GetProperty(FieldName).GetValue(BaseObj));
            SDKBusinessModel relBusinessModel = BackendRouterService.GetSDKBusinessModel(RelatedBusiness, null);
            var relBusinessType = Utilities.SearchType(relBusinessModel.Namespace + "." + relBusinessModel.Name);
            RelBusinessObj = ActivatorUtilities.CreateInstance(ServiceProvider, relBusinessType);
            if(AuthenticationService.User!=null){
                RowidCulture = AuthenticationService.User.RowidCulture;
            }
            if(RelatedParams != null){
                FieldTemplate = RelatedParams.FieldTemplate;
            }
            await LoadData("", null);
        }

        private void OnFocus(){
            isFocused = true;
            classList = "whcm_select_list";
            StateHasChanged();
        }
        private void OnBlur(){
            isFocused = false;
            classList = "whcm_select_list d-none";
            StateHasChanged();
        }

        private void OnSelectItem(dynamic item)
        {
            SetVal(item);
            LoadData("", null);
            if(OnChange != null){
                OnChange();
            }
            OnBlur();
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

        private void OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {

                if (CacheLoadResult != null && CacheLoadResult.data != null)
                {
                    var results = CacheLoadResult.data as IEnumerable<dynamic>;

                    if (results.Count() > 0)
                    {
                        SetVal(results.First());
                        LoadData("", null);
                        OnBlur();
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
        private async Task<LoadResult> LoadData(string searchText, CancellationToken? cancellationToken)
        {
            if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested)
            {
                return CacheLoadResult;
            }
            if (LastSearchString != searchText)
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
                if (BaseObj != null && BaseObj.Rowid != 0 && RelBusinessObj != null && RelBusinessObj.BaseObj != null && RelBusinessObj.BaseObj.GetType() == BaseObj.GetType())
                {
                    filters = $"(Rowid != {BaseObj.Rowid})";
                }
                foreach (var item in RelatedFilters)
                {
                    var value = item.Value;
                    var key = item.Key;
                    //remove "RelBaseObj." from key
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
                //Console.WriteLine($"filters: {searchText}");            
                var result = await RelBusinessObj.EntityFieldSearchAsync(searchText, filters); //TODO: Paginación
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
    }
}