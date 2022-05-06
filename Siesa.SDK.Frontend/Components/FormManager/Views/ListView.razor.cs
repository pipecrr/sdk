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
using System.Linq;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class ListView : ComponentBase
    {
        [Parameter]
        public string BusinessName { get; set; }
        [Parameter]
        public dynamic BusinessObj { get; set; }

        [Parameter]
        public bool SetTopBar { get; set; } = true;

        [Parameter]
        public bool IsSubpanel { get; set; }

        [Parameter]
        public List<string> ConstantFilters { get; set; } = new List<string>();

        [Parameter] 
        public bool AllowCreate { get; set; } = true;
        [Parameter] 
        public bool AllowEdit { get; set; } = true;
        [Parameter] 
        public bool AllowDelete { get; set; } = true;
        [Parameter] 
        public bool AllowDetail { get; set; } = true;

        [Inject] public IJSRuntime JSRuntime { get; set; }
        [Inject] public NavigationManager NavManager { get; set; }

        public bool Loading;

        public String ErrorMsg = "";
        private IList<object> SelectedObjects { get; set; }

        private ListViewModel ListViewModel { get; set; }

        [Parameter]
        public string ViewdefName { get; set; }

        [Parameter]
        public string DefaultViewdefName { get; set; } = "list";

        [Parameter]
        public Action<string> OnClickEdit { get; set; } = null;

        [Parameter]
        public Action<string> OnClickDetail { get; set; } = null;

        [Parameter]
        public Action<string,string> OnClickDelete { get; set; } = null;

        [Parameter]
        public Action OnClickNew { get; set; } = null;

        [Parameter]
        public Action<object> OnSelectedRow { get; set; } = null;

        private IEnumerable<object> data;
        int count;
        public RadzenDataGrid<object> _gridRef;

        Guid needUpdate;

        private void OnSelectionChanged(IList<object> objects){
            if(OnSelectedRow != null){
                SelectedObjects = objects;
                if (SelectedObjects?.Any() == true){
                    OnSelectedRow(objects.First());
                }else{
                    OnSelectedRow(null);
                }
                
            }
        }

        private string GetViewdef(string businessName)
        {
            var viewdef = "";
            if (String.IsNullOrEmpty(ViewdefName))
            {
                viewdef = DefaultViewdefName;
            }else{
                viewdef = ViewdefName;
            }

            var data = BusinessManagerFrontend.Instance.GetViewdef(businessName, viewdef);
            if (String.IsNullOrEmpty(data) && viewdef != DefaultViewdefName)
            {
                data = BusinessManagerFrontend.Instance.GetViewdef(businessName, DefaultViewdefName);
            }
            return data;
        }

        protected void InitView(string bName = null)
        {
            Loading = true;
            if (bName == null)
            {
                bName = BusinessName;
            }
            var metadata = GetViewdef(bName);
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
            var filters = $"{args.Filter}";
            if (ConstantFilters != null)
            {
                foreach (var filter in ConstantFilters)
                {
                    if (!string.IsNullOrEmpty(filters))
                    {
                        filters += " && ";
                    }
                    filters += $"{filter}";
                }
            }
            var dbData = await BusinessObj.GetDataAsync(args.Skip, args.Top, filters, args.OrderBy);
            data = dbData.Data;
            count = dbData.TotalCount;
            Loading = false;
        }

        private void GoToEdit(Int64 id)
        {
            if (OnClickEdit != null)
            {
                OnClickEdit(id.ToString());
            }
            else
            {
                NavManager.NavigateTo($"{BusinessName}/edit/{id}/");
            }
            
        }
        private void GoToCreate()
        {
            if (OnClickNew != null)
            {
                OnClickNew();
            }
            else
            {
                NavManager.NavigateTo($"{BusinessName}/create/");
            }
        }

        private void GoToDetail(Int64 id)
        {
            if (OnClickDetail != null)
            {
                OnClickDetail(id.ToString());
            }
            else
            {
                NavManager.NavigateTo($"{BusinessName}/detail/{id}/");
            }
        }

        private void GoToDelete(Int64 id, string object_string)
        {
            if (OnClickDelete != null)
            {
                OnClickDelete(id.ToString(), object_string);
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
