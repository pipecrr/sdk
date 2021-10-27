using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Microsoft.JSInterop;
using Siesa.SDK.Business;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DetailView: ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }

        [Inject] public IJSRuntime JSRuntime { get; set; }

        [Inject] public RefreshService RService { get; set; }
        protected List<Panel> Paneles = new List<Panel>();

        public Boolean ModelLoaded = false;

        public String ErrorMsg = "";

        protected void InitView(string bName = null) {
            if (bName == null) {
                bName = BusinessName;
            }
            var metadata = BusinessManager.Instance.GetViewdef(bName, "detail");
            if (metadata == null || metadata == "")
            {
                ErrorMsg = "No hay definición para la vista de detalle";
            }
            else
            {
                Paneles = JsonConvert.DeserializeObject<List<Panel>>(metadata);
                for (int i = 0; i < Paneles.Count; i++)
                {
                    for(int j = 0; j < Paneles[i].Fields.Count; j++)
                    {
                        Paneles[i].Fields[j].ViewContext = "DetailView";

                    }
                }
                ModelLoaded = true;
            }
            StateHasChanged();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (parameters.TryGetValue<string>(nameof(BusinessName), out var value))
            {
                if (value != null)
                {
                    this.ModelLoaded = false;
                    InitView(value);
                }
            }            
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            InitView();
        }
    }
}
