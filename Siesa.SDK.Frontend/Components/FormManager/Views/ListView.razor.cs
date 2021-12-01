using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Microsoft.JSInterop;
using Siesa.SDK.Business;
using DevExtreme.AspNet.Data.ResponseModel;
using DevExtreme.AspNet.Data;
using System.Threading;
using System.Reflection;
using DevExpress.Blazor;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Components.FormManager.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class ListView: ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }

        public Boolean Loading = true;

        public String ErrorMsg = "";

        private ListViewModel ListViewModel { get; set; }

        public DxDataGrid<object> _gridRef;
        protected void InitView(string bName = null) {
            Loading = true;
            if (bName == null) {
                bName = BusinessName;
            }
            var metadata = BusinessManagerFrontend.Instance.GetViewdef(bName, "list");
            if (metadata == null || metadata == "")
            {
                ErrorMsg = "No hay definición para la vista de lista";
            }
            else
            {
                ListViewModel = JsonConvert.DeserializeObject<ListViewModel>(metadata);
                foreach (var field in ListViewModel.Fields)
                {
                    field.InitField(BusinessObj);
                }
            }
            Loading = false;
            StateHasChanged();

        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //InitView();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (parameters.TryGetValue<string>(nameof(BusinessName), out var value))
            {
                if (value != null)
                {
                    Loading = false;
                    ErrorMsg = "";
                    InitView(value);
                }
            }            
        }

        protected async Task<LoadResult> LoadData(DataSourceLoadOptionsBase options, CancellationToken cancellationToken)
        {
            string tableOptions = options.ConvertToGetRequestUri("/");
            var result =  await BusinessObj.ListAsync(0,30,tableOptions); //TODO: Paginación
            var response = new LoadResult
            {
                data = result.Data,
                totalCount = result.TotalCount,
                groupCount = result.GroupCount
            };
            return response;

        }

        private RenderFragment BuildColumns()
        {
            RenderFragment columns = b =>
            {
                int counter = 0;
                foreach (var field in ListViewModel.Fields)
                {
                    
                    switch (field.FieldType)
                    {
                        case FieldTypes.CharField:
                        case FieldTypes.TextField:
                        case FieldTypes.EntityField:
                            b.OpenComponent(counter, typeof(DxDataGridColumn));
                            break;
                        case FieldTypes.DateField:
                        case FieldTypes.DateTimeField:
                            b.OpenComponent(counter, typeof(DxDataGridDateEditColumn));
                            break;
                        case FieldTypes.DecimalField:
                        case FieldTypes.IntegerField:
                            b.OpenComponent(counter, typeof(DxDataGridSpinEditColumn));
                            break;
                        case FieldTypes.BooleanField:
                            b.OpenComponent(counter, typeof(DxDataGridCheckBoxColumn));
                            break;

                        default:
                            continue;
                    
                        //default
                    }
                    var fieldName = field.Name;
                    //remove "BaseObj." from field name if exists
                    if (fieldName.StartsWith("BaseObj."))
                    {
                        fieldName = fieldName.Substring(8);
                    }
                    b.AddAttribute(0, "Field", fieldName);
                    b.AddAttribute(1, "Caption", field.Label);

                    if(field.Name == ListViewModel.LinkTo)
                    {
                        //TODO: Link al detalle
                    }
                    b.CloseComponent();
                    counter++;
                }
            };
            return columns;
        }

        private void GoToEdit(int id)
        {
            NavManager.NavigateTo($"{BusinessName}/edit/{id}/");
        }
        private void GoToCreate()
        {
            NavManager.NavigateTo($"{BusinessName}/create/");
        }

        private void GoToDetail(int id)
        {
            NavManager.NavigateTo($"{BusinessName}/detail/{id}/");
        }
    }
}
