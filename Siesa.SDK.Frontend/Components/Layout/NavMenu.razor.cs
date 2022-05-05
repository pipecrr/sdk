﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Shared.Backend;
using System.Linq;
using Siesa.SDK.Shared.Utilities;
using Siesa.SDK.Business;

namespace Siesa.SDK.Frontend.Components.Layout
{
    public partial class NavMenu : ComponentBase
    {
        [Inject]
        public IBackendManager backendManager { get; set; }
        public E00060_Suite SelectedSuite { get; set; }
        public List<E00061_Menu> Menus { get; set; }
        public List<E00061_Menu> DevMenu { get; set; }

        public string DevMenuSearchString { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Menus = new List<E00061_Menu>();
            DevMenu = new List<E00061_Menu>();
            _ = LoadMenu();
        }
        private async Task LoadMenu()
        {
            //TODO: Check Performance, Call only once
            backendManager.SyncWithMasterBackend();
            foreach (var backend in backendManager.GetBackendDict())
            {
                foreach (var business in backend.Value.businessRegisters.Businesses)
                {
                    BusinessManagerFrontend.Instance.AddBusiness(business, backend.Value.Name);
                    var businessType = Utils.Utils.SearchType(business.Namespace + "." + business.Name);
                    if (businessType == null)
                    {
                        continue;
                    }
                    var isBLExplorer = Utilities.IsAssignableToGenericType(businessType, typeof(BLFrontendExplorer<>));
                    if (isBLExplorer)
                    {
                        var customActionMenu = new E00061_Menu
                        {
                            //Title = business.Name,
                            Url = $"/{business.Name}/explorer/",
                            IconClass = "oi oi-project "
                        };
                        DevMenu.Add(customActionMenu);
                    }
                    else
                    {
                        var submenuItem = new E00061_Menu
                        {
                            //Title = business.Name,
                            IconClass = "oi oi-list-rich  menu-icon",
                            SubMenus = new List<E00061_Menu>{
                                new E00061_Menu{
                                    //Title = "Crear",
                                    Url = $"/{business.Name}/create/"
                                },
                                new E00061_Menu{
                                    //Title = "Consultar",
                                    Url = $"/{business.Name}/"
                                }
                            }
                        };
                        //search methods that return a RenderFragment
                        var customActions = businessType.GetMethods().Where(m => m.ReturnType == typeof(RenderFragment));
                        foreach (var customAction in customActions)
                        {
                            var customActionMenu = new E00061_Menu
                            {
                                //Title = customAction.Name,
                                Url = $"/{business.Name}/{customAction.Name}/",
                                IconClass = "oi oi-project "
                            };
                            submenuItem.SubMenus.Add(customActionMenu);
                        }

                        DevMenu.Add(submenuItem);
                    }



                }
            }
            var backends = backendManager.GetBackendDict();
            BackendRegistry backendRegistry = backends[backends.Keys.First()];
            backendRegistry.SetAuthenticationService(AuthenticationService);
            var jsonResponse = await backendRegistry.GetMenuGroupsAsync();
            SelectedSuite = Newtonsoft.Json.JsonConvert.DeserializeObject<List<E00060_Suite>>(jsonResponse.Response).First(); //TODO: UX difines how to select the menu group

            var menusJsonResponse = await backendRegistry.GetMenuItemsAsync(SelectedSuite.Rowid);
            Menus = Newtonsoft.Json.JsonConvert.DeserializeObject<List<E00061_Menu>>(menusJsonResponse.Response);
            Menus = Menus.OrderBy(x => x.Order).ToList();
            Console.WriteLine(Menus.Count);
            StateHasChanged();
        }

    }
}
