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
    public partial class CreateView : ComponentBase
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
        public string FormID { get; set; } = Guid.NewGuid().ToString();

        protected void InitView(string bName = null)
        {
            Loading = true;
            if (bName == null)
            {
                bName = BusinessName;
            }
            var metadata = BusinessManager.Instance.GetViewdef(bName, "create");
            if (metadata == null || metadata == "")
            {
                ErrorMsg = "No hay definición para la vista de creación";
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

        private async Task SaveBusiness()
        {
            Loading = true;
            StateHasChanged();
            //var id = await BusinessObj.SaveAsync();
            var result = await BusinessObj.ValidateAndSaveAsync();
            Loading = false;
            ErrorMsg = string.Empty;
            if (!result.Succesfull)
            {
                ErrorMsg = result.ResumeHTML();
               
                return;
            }
            var id = result.Rowid;
            NavManager.NavigateTo($"{BusinessName}/detail/{id}/");
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
