using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Shared.Backend;
using System.Linq;

namespace Siesa.SDK.Frontend.Components.Layout
{
    public partial class NavMenu : ComponentBase
    {
        [Inject]
        public IBackendManager backendManager { get; set; }
        public E00130_MenuGroup SelectedMenuGroup { get; set; }
        public List<E00131_Menu> Menus { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Menus = new List<E00131_Menu>();
            _ = LoadMenu();
        }
        private async Task LoadMenu()
        {
            //TODO: Check Performance, Call only once
            backendManager.SyncWithMasterBackend();  
            foreach(var backend in backendManager.GetBackendDict()){
                foreach(var business in backend.Value.businessRegisters.Businesses){
                    BusinessManagerFrontend.Instance.AddBusiness(business, backend.Value.Name);
                }
            }
            var backends = backendManager.GetBackendDict();
            BackendRegistry backendRegistry = backends[backends.Keys.First()];
            backendRegistry.SetAuthenticationService(AuthenticationService);
            var jsonResponse = await backendRegistry.GetMenuGroupsAsync();
            SelectedMenuGroup = Newtonsoft.Json.JsonConvert.DeserializeObject<List<E00130_MenuGroup>>(jsonResponse.Response).First(); //TODO: UX difines how to select the menu group

            var menusJsonResponse = await backendRegistry.GetMenuItemsAsync(SelectedMenuGroup.Rowid);
            Menus = Newtonsoft.Json.JsonConvert.DeserializeObject<List<E00131_Menu>>(menusJsonResponse.Response);
            Console.WriteLine(Menus.Count);
            StateHasChanged();
        }

    }
}
