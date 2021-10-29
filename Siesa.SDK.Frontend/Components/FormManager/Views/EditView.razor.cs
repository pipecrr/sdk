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
    public partial class EditView : ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }

        protected List<Panel> Paneles = new List<Panel>();

        public Boolean Loading = true;

        public String ErrorMsg = "";

        protected void InitView(string bName = null)
        {
            Loading = true;
            if (bName == null)
            {
                bName = BusinessName;
            }
            var metadata = BusinessManager.Instance.GetViewdef(bName, "edit");
            if (metadata == null || metadata == "")
            {
                ErrorMsg = "No hay definición para la vista de edición";
            }
            else
            {
                Paneles = JsonConvert.DeserializeObject<List<Panel>>(metadata);
            }
            Loading = false;
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            InitView();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (parameters.TryGetValue<string>(nameof(BusinessName), out var value))
            {
                if (value != null)
                {
                    Loading = false;
                    InitView(value);
                }
            }
        }

        private async Task SaveBusiness()
        {
            Loading = true;
            StateHasChanged();
            var id = await BusinessObj.SaveAsync();
            Loading = false;
            NavManager.NavigateTo($"{BusinessName}/detail/{id}/");
        }

        private void GoToList()
        {
            NavManager.NavigateTo($"{BusinessName}/");
        }

        async Task HandleValidSubmit()
        {
            ErrorMsg = @"Form data is valid";
            await SaveBusiness();
        }
        void HandleInvalidSubmit()
        {
            ErrorMsg = @"Form data is invalid";
        }
    }
}
