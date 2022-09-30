using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Blazor;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;

namespace Siesa.SDK.Frontend.Components.FormManager.Fields
{
    public partial class EntityField : FieldClass<dynamic>
    {
        [Inject] private IBackendRouterService BackendRouterService { get; set; }
        [Inject] private IServiceProvider ServiceProvider { get; set; }

        private RenderFragment? _fieldValidationTemplate;
        [Parameter] public dynamic BaseModelObj { get; set; }
        [Parameter] public int Version { get; set; } = 1;

        private LoadResult CacheLoadResult;
        private string LastSearchString;
        private int MinMillisecondsBetweenSearch = 100;
        private CancellationTokenSource cancellationTokenSource;

        private IEnumerable<dynamic> _data;
        private int count = 0;


        public dynamic RelBusinessObj { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }



        public static void EntitySetValue(EntityField field, dynamic value)
        {
            field.SetValue(value);
            try
            {
                if(!string.IsNullOrEmpty(field.FieldOpt.EntityRowidField) && value == null)
                {
                    field.SetValue(field.FieldOpt.EntityRowidField, value);
                }
            }
            catch (System.Exception)
            {
            }
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
            if (searchText.Length > FieldOpt.MinCharsEntityField || CacheLoadResult == null)
            {
                var filters = "";
                if (BaseModelObj != null && BaseModelObj.BaseObj != null && BaseModelObj.BaseObj.Rowid != 0 && RelBusinessObj != null && RelBusinessObj.BaseObj != null && RelBusinessObj.BaseObj.GetType() == BaseModelObj.BaseObj.GetType())
                {
                    filters = $"(Rowid != {BaseModelObj.BaseObj.Rowid})";
                }
                foreach (var item in FieldOpt.RelatedFilters)
                {
                    var value = item.Value;
                    var key = item.Key;
                    //remove "RelBaseObj." from key
                    key = key.Replace("RelBaseObj.", "");
                    dynamic dynamicValue;
                    try
                    {
                        dynamicValue = await Utils.Evaluator.EvaluateCode(value, BaseModelObj);
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
                CacheLoadResult = response;
                return response;
            }
            else
            {
                return CacheLoadResult;
            }
        }
        protected async Task<LoadResult> LoadCustomData(DataSourceLoadOptionsBase options, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            try
            {
                string tableOptions = options.ConvertToGetRequestUri("/");
                if (cancellationTokenSource != null &&
                !cancellationTokenSource.IsCancellationRequested)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource?.Dispose();
                }
                cancellationTokenSource = new CancellationTokenSource();
                var searchText = "";
                if (options.Filter != null)
                {
                    searchText = Convert.ToString(((List<object>)options.Filter[0])[2]);
                    //trim whitespace
                    searchText = searchText.Trim();
                }

                return await LoadData(searchText, cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = null;
                }
            }
            return CacheLoadResult;
        }

        private async Task LoadCustomDataRadzen(LoadDataArgs args)
        {
            try
            {
                var searchText = "";
                if (!string.IsNullOrEmpty(args.Filter))
                {
                    searchText = args.Filter.ToLower();
                    searchText = searchText.Trim();
                }

                var response = await LoadData(searchText, null);
                if (response != null)
                {
                    _data = response.data as IEnumerable<dynamic>;
                    count = response.totalCount;
                }
                StateHasChanged();



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void onKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {

                if (CacheLoadResult != null && CacheLoadResult.data != null)
                {
                    var results = CacheLoadResult.data as IEnumerable<dynamic>;

                    if (results.Count() > 0)
                    {
                        SetValue(results.First());
                    }
                }
                StateHasChanged();
            }
        }

        public RenderFragment? FieldTemplate
        {
            get
            {

                return _fieldValidationTemplate != null ? _fieldValidationTemplate : builder =>
                {
                    SDKBusinessModel relBusinessModel = BackendRouterService.GetSDKBusinessModel(FieldOpt.RelatedBusiness, null);
                    if (relBusinessModel == null)
                    {
                        throw new Exception("Business not found");
                    }

                    var relBusinessType = Utilities.SearchType(relBusinessModel.Namespace + "." + relBusinessModel.Name);
                    RelBusinessObj = ActivatorUtilities.CreateInstance(ServiceProvider, relBusinessType);
                    RelBusinessObj.BusinessName = FieldOpt.RelatedBusiness;
                    var access = Expression.Property(Expression.Constant(BindModel, BindModel.GetType()), FieldName);
                    var lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(BindProperty.PropertyType), access);



                    var dxEventCallbackMethod = typeof(EventCallbackFactory).GetMethods()[8]; //TODO: traer por medio de la firma {Microsoft.AspNetCore.Components.EventCallback`1[TValue] Create[TValue](System.Object, System.Action`1[TValue])}
                    var dxEventCallback = dxEventCallbackMethod.MakeGenericMethod(BindProperty.PropertyType);


                    var lambdadxValue = Expression.Parameter(BindProperty.PropertyType, "t");
                    var thisExpresionConstant = Expression.Constant(this, this.GetType());
                    var body = Expression.Call(null, this.GetType().GetMethod("EntitySetValue"), thisExpresionConstant, lambdadxValue);
                    var actionGenericType = typeof(Action<>).MakeGenericType(BindProperty.PropertyType);

                    var eventCallbackLambda = Expression.Lambda(actionGenericType, body, lambdadxValue).Compile();
                    //var eventCallbackLambda = Expression.Lambda(Expression.Call(thisParam, this.GetType().GetMethod("SetValue"), lambdaDxValue), lambdaDxValue, thisParam);
                    var dxEventCallbackCall = dxEventCallback.Invoke(EventCallback.Factory, new object[] { this, eventCallbackLambda });

                    //MethodInfo generic = dxEventCallbackMethod.MakeGenericMethod(dxType);
                    //generic.Invoke(this, null);

                    var dxValue = (BindValue != null ? Convert.ChangeType(BindValue, BindProperty.PropertyType) : null);

                    /*//Create an empty List of type BindModel.GetType()
                    var dataList = Activator.CreateInstance(typeof(List<>).MakeGenericType(BindModel.GetType()));


                    //Add the value to the list if it is not null
                    if (BindValue != null)
                    {
                        ((dynamic)dataList).Add(dxValue);
                    }*/
                    if (Version == 1)
                    {
                        Type dxType = typeof(DxComboBox<,>).MakeGenericType(BindProperty.PropertyType, BindProperty.PropertyType);

                        builder.OpenComponent(0, dxType);
                        builder.AddAttribute(1, "ValueExpression", lambda);
                        builder.AddAttribute(2, "Value", (object)dxValue);
                        //builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<dynamic>(this, str => { SetValue(str); }));
                        builder.AddAttribute(3, "ValueChanged", dxEventCallbackCall);
                        builder.AddAttribute(4, "NullText", FieldOpt.Placeholder);
                        builder.AddAttribute(5, "CssClass", $"cw-480 {FieldOpt.CssClass}");
                        builder.AddAttribute(5, "Enabled", !FieldOpt.Disabled);
                        builder.AddAttribute(6, "ClearButtonDisplayMode", DataEditorClearButtonDisplayMode.Auto);
                        builder.AddAttribute(7, "field-name", FieldOpt.Name);
                        builder.AddAttribute(8, "FilteringMode", DataGridFilteringMode.Contains);
                        //builder.AddAttribute(9, "Data", dataList);
                        builder.AddAttribute(9, "CustomData", LoadCustomData);
                        builder.AddAttribute(10, "onkeydown", onKeyDown);
                        builder.CloseComponent();
                    }
                    else
                    {
                        Type radzenType = typeof(RadzenDropDown<>).MakeGenericType(BindProperty.PropertyType);
                        builder.OpenComponent(0, radzenType);
                        builder.AddAttribute(1, "AllowClear", true);
                        builder.AddAttribute(2, "AllowVirtualization", true);
                        builder.AddAttribute(3, "AllowFiltering", true);
                        builder.AddAttribute(4, "Class", "w-100");
                        builder.AddAttribute(5, "Change", EventCallback.Factory.Create<object>(this, str => { SetValue(str); }));
                        builder.AddAttribute(6, "Data", _data);
                        builder.AddAttribute(7, "field-name", FieldOpt.Name);
                        builder.AddAttribute(8, "Value", (object)dxValue);
                        builder.AddAttribute(9, "LoadData", EventCallback.Factory.Create<LoadDataArgs>(this, async args => { await LoadCustomDataRadzen(args); }));
                        builder.AddAttribute(10, "Count", count);
                        builder.AddAttribute(11, "FilterDelay", 150);
                        builder.AddAttribute(12, "ReadOnly", FieldOpt.Disabled);
                        builder.AddAttribute(13, "OpenOnFocus", true);

                        builder.CloseComponent();

                    }

                };

            }
        }
    }
}