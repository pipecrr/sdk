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
    public partial class SDKManyToManySelector
    {
        [Parameter] public string RelatedBusiness {get; set;}
        [Parameter] public List<int> RowidRecordsRelated {get; set;}
        [Parameter] public string RemoveButtonResourceTag {get; set;} = "Remove items";
        [Parameter] public string AddButtonResourceTag {get; set;} = "Add items";
        
        

        [Parameter] public EventCallback<List<int>> OnAddUserAction {get; set;}
        [Parameter] public string ListResourceTag {get; set;}
        [Parameter] public string EmptyUsersResourceTag {get; set;}
        [Parameter] public Action<List<int>> OnFixedClick {get; set;}
        [Inject] public SDKGlobalLoaderService LoaderService {get; set;}
        [Inject] public SDKNotificationService NotificationService {get; set;}
        [Inject] public IBackendRouterService BackendRouterService { get; set; }
        [Inject] public IServiceProvider ServiceProvider { get; set; }
        private SDKFixedButtonManyToMany ComponentFixedButtonRef {get; set;}
        private ListView ListViewRef {get; set;}
        private SDKEntityField _sdkEntityFieldRef {get;set;}
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
            var relBusinessModel = BackendRouterService.GetSDKBusinessModel(RelatedBusiness, null);
            var relBusinessType = Utilities.SearchType(relBusinessModel.Namespace + "." + relBusinessModel.Name);
            BusinessRelated = ActivatorUtilities.CreateInstance(ServiceProvider, relBusinessType);
            SetFilter();
            //Business.UsersToOperate = new();
        }

        private void SetNotIn()
        {
            EntityFieldFilters.Clear();

            if (RowidRecordsRelated is null) return;

            var Filter = new { Rowid__notin = RowidRecordsRelated };
            
            EntityFieldFilters.Add(new List<object>(){Filter});
        }

        private void SetFilter()
        {
            if(RowidRecordsRelated is not null && RowidRecordsRelated.Any()){
                ConstantFilters = AddConstantFilters(RowidRecordsRelated);
            }
            /*if(Users is not null && Users.Any())
                Business.Users = Users;
            else
                Business.Users = new();
*/
            SetNotIn();
            
            if (RowidRecordsRelated is null) return;

            var Filter = string.Empty;
            //UserComponentUtil.SetFilter(ConstantFilters, Users, ref Filter);
            LastFilter = Filter;
            
            StateHasChanged();
        }
        
        private List<string> AddConstantFilters(List<int> rowidUsers)
        {
            var constantFilters = new List<string>();
            var filter = rowidUsers.Select(x => $"Rowid = {x}");
            constantFilters.Add($"({string.Join(" || ", filter)})");
            return constantFilters;
        }

        public async Task RefreshListView()
        {
            SetFilter();

            if (ListViewRef is not null)
            {
                _ = ListViewRef.Refresh(true);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            //var CurrentFilter = Users is null ? string.Empty : UserComponentUtil.GetFilter(Users);
            
            /*if (string.Equals(LastFilter, CurrentFilter, StringComparison.OrdinalIgnoreCase))
                return;
*/
            RefreshListView();

            // if(UserComponentFixedButtonRef != null)
            //     Business.UserComponentFixedButtonRef = UserComponentFixedButtonRef;

            //Business.UsersToOperate = new();
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
            var itemsSelected = _sdkEntityFieldRef.GetItemsSelected();
            if(!itemsSelected.Any())
            {
                _ = NotificationService.ShowInfo("Custom.BLUser.SelectUserMessage");
                return;
            }

            var rowids = itemsSelected.Where(x => !RowidRecordsRelated.Any(y=>y==x.Rowid))
                            .Select(x => (int) x.Rowid).ToList();

            if(!rowids.Any())
            {
                _ = NotificationService.ShowError("Custom.BLUser.ErrorToAssignUser");
                return;
            }

            if (OnAddUserAction.HasDelegate)
            {
                OnAddUserAction.InvokeAsync(rowids);
            }
            
            RowidRecordsRelated.AddRange(rowids);
            _ = _sdkEntityFieldRef.Clean();
            _ = RefreshListView();
        }

        private void FixedClick()
        {
            if (OnFixedClick is not null)
            {
                OnFixedClick(ItemsSelected);
            }
            RowidRecordsRelated = RowidRecordsRelated.Where(x => !ItemsSelected.Any(y => y == x)).ToList();
            _ = RefreshListView();
        }
    }
}