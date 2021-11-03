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

        protected void InitView(string bName = null) {
            Loading = true;
            if (bName == null) {
                bName = BusinessName;
            }
            var metadata = BusinessManager.Instance.GetViewdef(bName, "list");
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
            await Task.Delay(5000);
            string tableOptions = options.ConvertToGetRequestUri("/");
            /*using var response = await Client.GetAsync(options.ConvertToGetRequestUri(dataEndpointUri), cancellationToken);
            response.EnsureSuccessStatusCode();*/
            //using var responseStream = await response.Content.ReadAsStreamAsync();
            var responseStream = "{}";
            return await Task.Run(() => JsonConvert.DeserializeObject<LoadResult>(responseStream), cancellationToken: cancellationToken);
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
                   
                    b.AddAttribute(0, "Field", field.Name);
                    b.AddAttribute(1, "Caption", field.Label);
                    b.CloseComponent();
                    counter++;
                }
            };
            return columns;
        }
    }
}
