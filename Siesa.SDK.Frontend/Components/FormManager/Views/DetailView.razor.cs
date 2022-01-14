using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Microsoft.JSInterop;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DetailView: ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }

        protected List<Panel> Paneles = new List<Panel>();

        public Boolean ModelLoaded = false;

        public String ErrorMsg = "";

        private void setViewContext(List<Panel> panels) { 
            for (int i = 0; i < panels.Count; i++)
            {
                for(int j = 0; j < panels[i].Fields.Count; j++)
                {
                    panels[i].Fields[j].ViewContext = "DetailView";
                }
                if(panels[i].SubViewdef != null && panels[i].SubViewdef.Paneles.Count > 0)
                {
                    setViewContext(panels[i].SubViewdef.Paneles);
                }
            }

        }

        protected void InitView(string bName = null) {
            if (bName == null) {
                bName = BusinessName;
            }
            var metadata = BusinessManagerFrontend.Instance.GetViewdef(bName, "detail");
            if (metadata == null || metadata == "")
            {
                ErrorMsg = "No hay definición para la vista de detalle";
            }
            else
            {
                Paneles = JsonConvert.DeserializeObject<List<Panel>>(metadata);
                setViewContext(Paneles);
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
                    ErrorMsg = "";
                    InitView(value);
                }
            }            
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //InitView();
        }

        private void GoToEdit()
        {
            NavManager.NavigateTo($"{BusinessName}/edit/{BusinessObj.BaseObj.Rowid}/");
        }

        private void GoToList()
        {
            NavManager.NavigateTo($"{BusinessName}/");
        }

        private async Task DeleteBusiness()
        {
            await BusinessObj.DeleteAsync();
            NavManager.NavigateTo($"{BusinessName}/");
        }
    }
}
