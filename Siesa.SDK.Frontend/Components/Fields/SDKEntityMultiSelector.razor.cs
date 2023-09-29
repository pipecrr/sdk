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
    /// <summary>
    /// Represents a multi-selector component for entities.
    /// </summary>
    public partial class SDKEntityMultiSelector
    {   
        /// <summary>
        /// Gets or sets the related business name.
        /// </summary>
        [Parameter] public string RelatedBusiness {get; set;}
        /// <summary>
        /// Gets or sets the list of Rowids for related records.
        /// </summary>
        [Parameter] public List<int> RowidRecordsRelated {get; set;}
        /// <summary>
        /// Gets or sets the name of the view definition, optional.
        /// </summary>
        [Parameter] public string ViewdefName {get; set;}
        /// <summary>
        /// Gets or sets the resource tag for the Remove button.
        /// </summary>
        [Parameter] public string RemoveButtonResourceTag {get; set;} = "Custom.SDKEntityMultiSelector.RemoveItems";
        /// <summary>
        /// Gets or sets the resource tag for the Add button.
        /// </summary>
        [Parameter] public string AddButtonResourceTag {get; set;} = "Custom.SDKEntityMultiSelector.AddItems";
        /// <summary>
        /// Gets or sets the resource tag for the Add button in modal.
        /// </summary>
        [Parameter] public string AddButtonResourceTagMulti {get; set;}
        /// <summary>
        /// Gets or sets the resource tag for the label, default is the related business plural.
        /// </summary>
        [Parameter] public string LabelResourceTag { get; set; }
        /// <summary>
        /// Gets or sets the event callback for the add action, default add elements to the list of Rowids.
        /// </summary>
        [Parameter] public EventCallback<List<int>> OnAddAction {get; set;}
        /// <summary>
        /// Gets or sets the action for the add action in modal, default add elements to the list of Rowids.
        /// </summary>
        [Parameter] public Action<List<int>> OnAddActionModal {get; set;}
        /// <summary>
        /// Gets or sets the action for the remove action.
        /// </summary>
        [Parameter] public Action<List<int>> OnRemoveAction {get; set;}
        /// <summary>
        /// Gets or sets the resource tag for the list, default not set.
        /// </summary>
        [Parameter] public string ListResourceTag {get; set;}
        /// <summary>
        /// Gets or sets the resource tag for empty content.
        /// </summary>
        [Parameter] public string EmptyResourceTag {get; set;} = "Custom.SDKEntityMultiSelector.Empty";
        /// <summary>
        /// Gets or sets the SDKGlobalLoaderService dependency.
        /// </summary>
        [Inject] public SDKGlobalLoaderService LoaderService {get; set;}
        /// <summary>
        /// Gets or sets the SDKNotificationService dependency.
        /// </summary>
        [Inject] public SDKNotificationService NotificationService {get; set;}
        /// <summary>
        /// Gets or sets the IBackendRouterService dependency.
        /// </summary>
        [Inject] public IBackendRouterService BackendRouterService { get; set; }
        /// <summary>
        /// Gets or sets the IServiceProvider dependency.
        /// </summary>
        [Inject] public IServiceProvider ServiceProvider { get; set; }

        private SDKEntityMultiSelectorFixedButton ComponentFixedButtonRef {get; set;}
        private ListView ListViewRef {get; set;}
        private SDKEntityField SdkEntityFieldRef {get;set;}
        private List<string> ConstantFilters { get; set; } = new();
        private string LastFilter { get; set; }
        private List<List<object>> EntityFieldFilters { get; set; } = new(); 
        private bool ShowList { get; set; } = true;
        private dynamic BusinessRelated {get; set;}
        /// <summary>
        /// Gets or sets the list of rowid items selected to modal.
        /// </summary>
        public List<int> ItemsSelected {get; set;}
        /// <summary>
        /// Gets or sets the show button remove.
        /// </summary>
        public bool ShowButtonRemove {get; set;}
        /// <summary>
        /// Name of the field to get value from entity field component.
        /// </summary>
        public List<dynamic> FieldRelated {get; set;} = new();

        /// <summary>
        /// Initializes the component asynchronously.
        /// </summary>
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
            if (RowidRecordsRelated is null || !RowidRecordsRelated.Any()) return;
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

        /// <summary>
        /// Refreshes the list view with updated data.
        /// </summary>
        public void RefreshListView()
        {
            SetFilter();
            if (ListViewRef is not null)
            {
                _ = ListViewRef.Refresh(true);
            }
        }
        /// <summary>
        /// This method is called when the component is ready to start, having received its initial parameters from its parent in the render tree.
        /// </summary>        
        protected override async Task OnParametersSetAsync()
        {
            RefreshListView();
            await base.OnParametersSetAsync().ConfigureAwait(true);
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
            if (OnAddAction.HasDelegate)
            {
                OnAddAction.InvokeAsync(rowids);
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
        /// <summary>
        /// Closes the modal dialog.
        /// </summary>
        public void CloseModal()
        {
            SDKDialogService.Close();
            RefreshListView();
        }
    }
}