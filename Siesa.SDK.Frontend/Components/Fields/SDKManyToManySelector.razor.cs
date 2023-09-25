using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Views;
using Siesa.SDK.Frontend.Services;

namespace Siesa.SDK.Frontend.Components.Fields
{
    public partial class SDKManyToManySelector
    {
        [Parameter] public dynamic Business {get; set;}
        [Parameter] public EventCallback<List<int>> OnAddUserAction {get; set;}
        [Parameter] public string ListResourceTag {get; set;}
        [Parameter] public string EmptyUsersResourceTag {get; set;}
        [Parameter] public string FixedButtonResourceTag {get; set;}
        [Parameter] public List<int> Users {get; set;}
        [Parameter] public Action<List<int>> OnFixedClick {get; set;}
        [Inject] public SDKGlobalLoaderService LoaderService {get; set;}
        [Inject] public SDKNotificationService NotificationService {get; set;}
        //private UserComponentFixedButton UserComponentFixedButtonRef {get; set;}
        private ListView ListViewRef {get; set;}
        private SDKEntityField _sdkEntityFieldRef {get;set;}
        private List<string> ConstantFilters { get; set; } = new();
        private string LastFilter { get; set; }
        private List<List<object>> EntityFieldFilters { get; set; } = new(); 
        private bool ShowList { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            SetFilter();
            //Business.UsersToOperate = new();
            await base.OnInitializedAsync();
        }

        private void SetNotIn()
        {
            EntityFieldFilters.Clear();

            if (Users is null) return;

            var Filter = new { Rowid__notin = Users };
            
            EntityFieldFilters.Add(new List<object>(){Filter});
        }

        private void SetFilter()
        {
            /*if(Users is not null && Users.Any())
                Business.Users = Users;
            else
                Business.Users = new();
*/
            SetNotIn();
            
            if (Users is null) return;

            var Filter = string.Empty;
            //UserComponentUtil.SetFilter(ConstantFilters, Users, ref Filter);
            LastFilter = Filter;
            
            StateHasChanged();
        }

        public void RefreshListView()
        {
            Business.Users = Users;
            
            SetFilter();

            if(ListViewRef is not null)
                _ = ListViewRef.Refresh(true);
        }

        protected override async Task OnParametersSetAsync()
        {
            //var CurrentFilter = Users is null ? string.Empty : UserComponentUtil.GetFilter(Users);
            
            /*if (string.Equals(LastFilter, CurrentFilter, StringComparison.OrdinalIgnoreCase))
                return;
*/
            SetFilter();

            if (ListViewRef is not null)
            {
                //Esto se hace porque el refresh del Listview no refrezca a la flex
                //TODO: Quitar este crimen cuando SDK solucione el issue #336, descomentar linea 92
                if (ListViewRef.UseFlex)
                {
                    ShowList = false;
                    StateHasChanged();
                    await Task.Delay(2);
                    ShowList = true;
                }
                else
                    _ = ListViewRef.Refresh(true);
                
                //_ = ListViewRef.Refresh(true);
            }

            // if(UserComponentFixedButtonRef != null)
            //     Business.UserComponentFixedButtonRef = UserComponentFixedButtonRef;

            //Business.UsersToOperate = new();

            await base.OnParametersSetAsync();
        }

        private void OnSelectRow(IList<dynamic> Users)
        {
            /*if (Users is null)
            {
                Business.UsersToOperate = new();
                UserComponentFixedButtonRef?.Refresh();
                return;
            }
            Business.UsersToOperate = Users.Select(x => (int) x.GetType().GetProperty("Rowid").GetValue(x))
                .ToList();
            UserComponentFixedButtonRef?.Refresh();*/
        }

        private void OnAddUserClick()
        {
            var UsersSelected = _sdkEntityFieldRef.GetItemsSelected();

            if(!UsersSelected.Any())
            {
                _ = NotificationService.ShowInfo("Custom.BLUser.SelectUserMessage");
                return;
            }

            var Rowids = UsersSelected
                            .Where(x => !Users.Any(y=>y==x.Rowid))
                            .Select(x => (int) x.Rowid).ToList();

            if(!Rowids.Any())
            {
                _ = NotificationService.ShowError("Custom.BLUser.ErrorToAssignUser");
                return;
            }

            if(!OnAddUserAction.HasDelegate) return;

            OnAddUserAction.InvokeAsync(Rowids);
        }
    }
}