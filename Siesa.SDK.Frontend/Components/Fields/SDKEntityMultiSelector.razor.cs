using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Frontend.Components.FormManager.Views;
using Siesa.SDK.Frontend.Components.Visualization;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;

namespace Siesa.SDK.Frontend.Components.Fields
{
    public partial class SDKEntityMultiSelector
    {
        [Parameter] public string RelatedBusiness {get; set;}
        [Parameter] public List<int> RowidRecordsRelated {get; set;}
        [Parameter] public string ViewdefName {get; set;}
        [Parameter] public string RemoveButtonResourceTag {get; set;} = "Custom.SDKEntityMultiSelector.RemoveItems";
        [Parameter] public string AddButtonResourceTag {get; set;} = "Custom.SDKEntityMultiSelector.AddItems";
        [Parameter] public string AddButtonResourceTagMulti {get; set;}
        [Parameter] public string LabelResourceTag { get; set; }
        [Parameter] public Action<List<int>> OnAddAction {get; set;}
        [Parameter] public Action<List<int>> OnAddActionModal {get; set;}
        [Parameter] public Action<List<int>> OnRemoveAction {get; set;}
        [Parameter] public string ListResourceTag {get; set;}
        [Parameter] public string EmptyResourceTag {get; set;}
        [Inject] public SDKGlobalLoaderService LoaderService {get; set;}
        [Inject] public SDKNotificationService NotificationService {get; set;}
        [Inject] public IBackendRouterService BackendRouterService { get; set; }
        [Inject] public IServiceProvider ServiceProvider { get; set; }
        private SDKEntityMultiSelectorFixedButton ComponentFixedButtonRef {get; set;}
        private ListView ListViewRef {get; set;}
        private SDKEntityField SdkEntityFieldRef {get;set;}
        private List<string> ConstantFilters { get; set; } = new();
        private string LastFilter { get; set; }
        private List<List<object>> EntityFieldFilters { get; set; } = new(); 
        private bool ShowList { get; set; } = true;
        public List<int> ItemsSelected {get; set;}
        public bool ShowButtonRemove {get; set;}
        private dynamic BusinessRelated {get; set;}
        
        public List<dynamic> FieldRelated {get; set;}

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(LabelResourceTag))
            {
                LabelResourceTag = $"{RelatedBusiness}.Plural";
            }
            if (string.IsNullOrEmpty(AddButtonResourceTagMulti))
            {
                AddButtonResourceTagMulti = "Custom.SDKEntityMultiSelector.AddItems";
            }
            var relBusinessModel = BackendRouterService.GetSDKBusinessModel(RelatedBusiness, null);
            var relBusinessType = Utilities.SearchType(relBusinessModel.Namespace + "." + relBusinessModel.Name);
            BusinessRelated = ActivatorUtilities.CreateInstance(ServiceProvider, relBusinessType);
            SetFilter();
            await base.OnInitializedAsync().ConfigureAwait(true);
        }

        private void SetNotIn()
        {
            EntityFieldFilters.Clear();
            if (RowidRecordsRelated is null) return;
            var filter = new { Rowid__notin = RowidRecordsRelated };
            EntityFieldFilters.Add(new List<object>(){filter});
        }

        private void SetFilter()
        {
            if(RowidRecordsRelated is not null && RowidRecordsRelated.Any()){
                ConstantFilters = AddConstantFilters(RowidRecordsRelated);
            }
            SetNotIn();
            if (RowidRecordsRelated is null) return;
            var filter = string.Empty;
            LastFilter = filter;
            StateHasChanged();
        }
        
        private static List<string> AddConstantFilters(List<int> rowidItems)
        {
            var constantFilters = new List<string>();
            var filter = rowidItems.Select(x => $"Rowid = {x}");
            constantFilters.Add($"({string.Join(" || ", filter)})");
            return constantFilters;
        }

        public void RefreshListView()
        {
            SetFilter();
            if (ListViewRef is not null)
            {
                _ = ListViewRef.Refresh(true);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            RefreshListView();
            await base.OnParametersSetAsync();
        }

        private void OnSelectRow(IList<dynamic> items)
        {
            ItemsSelected = items.Select(x => (int) x.GetType().GetProperty("Rowid").GetValue(x)).ToList();
            ShowButtonRemove = ItemsSelected.Any();
            StateHasChanged();
        }

        private void AddItem()
        {
            var itemsSelected = SdkEntityFieldRef.GetItemsSelected();
            if(!itemsSelected.Any())
            {
                _ = NotificationService.ShowInfo("Custom.SDKEntityMultiSelector.SelectMessage");
                return;
            }
            var rowids = itemsSelected.Where(x => !RowidRecordsRelated.Any(y=>y==x.Rowid))
                            .Select(x => (int) x.Rowid).ToList();
            if(!rowids.Any())
            {
                _ = NotificationService.ShowError("Custom.SDKEntityMultiSelector.ErrorToAssign");
                return;
            }
            if (OnAddAction is not null)
            {
                OnAddAction(rowids);
            }
            RowidRecordsRelated.AddRange(rowids);
            _ = SdkEntityFieldRef.Clean();
            RefreshListView();
        }

        private void FixedClick()
        {
            if (OnRemoveAction is not null)
            {
                OnRemoveAction(ItemsSelected);
            }
            RowidRecordsRelated = RowidRecordsRelated.Where(x => !ItemsSelected.Any(y => y == x)).ToList();
            RefreshListView();
        }
        
        public void CloseModal()
        {
            SDKDialogService.Close();
            RefreshListView();
        }
    }
}