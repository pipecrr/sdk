using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Microsoft.JSInterop;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Utils;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DetailView : ComponentBase
    {
        [Inject] public  Radzen.DialogService dialogService { get; set; }
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }

        [Parameter]
        public bool SetTopBar { get; set; } = true;

        [Parameter] 
        public bool IsSubpanel { get; set; }

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }


        protected FormViewModel FormViewModel { get; set; } = new FormViewModel();
        protected List<Panel> Panels { get { return FormViewModel.Panels; } }

        public Boolean ModelLoaded = false;

        public String ErrorMsg = "";

        private void setViewContext(List<Panel> panels)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                for (int j = 0; j < panels[i].Fields.Count; j++)
                {
                    panels[i].Fields[j].ViewContext = "DetailView";
                }
                if (panels[i].SubViewdef != null && panels[i].SubViewdef.Panels.Count > 0)
                {
                    setViewContext(panels[i].SubViewdef.Panels);
                }
            }

        }

        protected void InitView(string bName = null)
        {
            if (bName == null)
            {
                bName = BusinessName;
            }
            var metadata = BusinessManagerFrontend.Instance.GetViewdef(bName, "detail");
            if (metadata == null || metadata == "")
            {
                ErrorMsg = "No hay definición para la vista de detalle";
            }
            else
            {
                try
                {
                    FormViewModel = JsonConvert.DeserializeObject<FormViewModel>(metadata);
                }
                catch (System.Exception)
                {
                    //Soporte a viewdefs anteriores
                    var panels = JsonConvert.DeserializeObject<List<Panel>>(metadata);
                    FormViewModel.Panels = panels;
                }
                setViewContext(Panels);
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
        private void GoToCreate()
        {
            NavManager.NavigateTo($"{BusinessName}/create/");
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
            if(IsSubpanel){
                dialogService.Close(false);
            }else{
                NavManager.NavigateTo($"{BusinessName}/");
            }
        }

        private void OnClickCustomButton(Button button)
        {
            if (!string.IsNullOrEmpty(button.Href))
            {
                if (button.Target == "_blank")
                {
                    _ = JSRuntime.InvokeVoidAsync("window.open", button.Href, "_blank");
                }
                else
                {
                    NavManager.NavigateTo(button.Href);
                }


            }
            else if (!string.IsNullOrEmpty(button.Action))
            {
                Evaluator.EvaluateCode(button.Action, BusinessObj);
            }
        }
    }
}
