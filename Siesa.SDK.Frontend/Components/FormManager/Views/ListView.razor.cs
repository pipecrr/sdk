using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Microsoft.JSInterop;
using Siesa.SDK.Business;
using System.Threading;
using System.Reflection;
using Siesa.SDK.Frontend.Components.FormManager.Model.Fields;
using Siesa.SDK.Frontend.Components.FormManager.Fields;
using Radzen;
using Radzen.Blazor;
using Siesa.SDK.Frontend.Utils;
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

        public bool Loading;

        public String ErrorMsg = "";

        private ListViewModel ListViewModel { get; set; }


        private IEnumerable<object> data;
        int count;
        public RadzenDataGrid<object> _gridRef;

        Guid needUpdate;
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
            data = null;
            Loading = false;
            StateHasChanged();

        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //InitView();
        }

        protected override void OnParametersSet()
        {
            Loading = false;
            ErrorMsg = "";
            InitView();
            data = null;
            if (_gridRef != null)
            {
                needUpdate = Guid.NewGuid();
                _gridRef.Reload();
            }
            StateHasChanged();      
        }

        async Task LoadData(LoadDataArgs args)
        {
            Loading = true;
            data = null;
            var skip = args.Skip;
            var top = args.Top;
            // var dbData = await TableViewObj.GetData(skip,top, args.Filter, args.OrderBy);
            var dbData = await BusinessObj.GetDataAsync(args.Skip,args.Top, args.Filter, args.OrderBy); //TODO: Paginación
            data = dbData.Data;
            count = dbData.TotalCount;
            Loading = false;
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
