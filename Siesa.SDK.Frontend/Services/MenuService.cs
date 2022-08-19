using Microsoft.Extensions.Hosting;
using Siesa.SDK.Frontend.Components.Layout.Header;
using Siesa.SDK.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Shared.Backend;
using System.Linq;
using Siesa.SDK.Shared.Utilities;
using Siesa.SDK.Business;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Components.Layout.NavMenu;
using Siesa.SDK.Frontend.Application;
using Siesa.SDK.Frontend.Components.Layout;
using Siesa.SDK.Frontend.Components.FormManager.Model;

namespace Siesa.SDK.Frontend.Services
{
    public class MenuService
    {
        public List<E00061_Menu> Menus { get; set; }
        private string environment;
        private E00060_Suite SelectedSuite { get; set; }
        private IBackendRouterService BackendRouterService;
        private IAuthenticationService AuthenticationService;
        private UtilsManager UtilsManager { get; set; }

        public MenuService(IBackendRouterService backendRouterService, IAuthenticationService authenticationService, UtilsManager utilsManager)
        {
            BackendRouterService = backendRouterService;
            AuthenticationService = authenticationService;
            UtilsManager = utilsManager;

            Menus = new List<E00061_Menu>();
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == Environments.Development)
            {
                Menus.Add(new E00061_Menu()
                {
                    ResourceTag = "DevMenu",
                    IconClass = "fa-solid fa-code",
                    SubMenus = new List<E00061_Menu>(),
                    CurrentText = "DevMenu",
                });
            }

            _ = LoadMenu();
        }
        public event EventHandler MenuLoaded
        {
            add
            {
                _menuLoaded += value;
            }
            remove
            {
                _menuLoaded -= value;
            }
        }

        private EventHandler? _menuLoaded;

        public async Task LoadMenu()
        {
            var DevMenu = Menus.FirstOrDefault(x => x.ResourceTag == "DevMenu")?.SubMenus;

            foreach (var business in BackendRouterService.GetBusinessModelList())
            {
                if (DevMenu == null)
                {
                    continue;
                }
                var businessType = Utilities.SearchType(business.Namespace + "." + business.Name);
                if (businessType == null)
                {
                    continue;
                }
                var isBLExplorer = Utilities.IsAssignableToGenericType(businessType, typeof(BLFrontendExplorer<>));
                if (isBLExplorer)
                {
                    var customActionMenu = new E00061_Menu
                    {
                        ResourceTag = $"{business.Name}.Plural",
                        Url = $"/{business.Name}/explorer/",
                    };
                    DevMenu.Add(customActionMenu);
                }
                else
                {
                    var submenuItem = new E00061_Menu
                    {
                        ResourceTag = $"{business.Name}.Plural",
                        Url = $"/{business.Name}/",
                        SubMenus = new List<E00061_Menu>()
                    };
                    //search methods that return a RenderFragment
                    var customActions = businessType.GetMethods().Where(m => m.ReturnType == typeof(RenderFragment));
                    foreach (var customAction in customActions)
                    {
                        var customActionMenu = new E00061_Menu
                        {
                            ResourceTag = $"{business.Name}.CustomAction.{customAction.Name}",
                            Url = $"/{business.Name}/{customAction.Name}/"
                        };
                        submenuItem.SubMenus.Add(customActionMenu);
                    }

                    DevMenu.Add(submenuItem);
                }
            }


            var menuBL = BackendRouterService.GetSDKBusinessModel("BLAdminMenu", AuthenticationService);
            var request = await menuBL.Call("GetSuites");
            if (request.Success)
            {
                SelectedSuite = ((List<E00060_Suite>)request.Data).First();
                var menuRequest = await menuBL.Call("GetMenuItems", Convert.ToInt64(SelectedSuite.Rowid));
                if (menuRequest.Success)
                {
                    List<E00061_Menu> menuResponse = menuRequest.Data;
                    menuResponse = menuResponse.OrderBy(x => x.Order).ToList();
                    //add menuResponse to Menus
                    Menus.AddRange(menuResponse);
                    GetMenuResources(Menus);
                    NotifyMenuLoaded();
                }
            }
        }


        private async Task GetMenuResources(ICollection<E00061_Menu> _menus)
        {
            foreach (var menuItem in _menus)
            {
                if ((menuItem.RowidResource == null || menuItem.RowidResource == 0) && menuItem.ResourceTag != null)
                {
                    menuItem.CurrentText = await UtilsManager.GetResource(menuItem.ResourceTag);
                }
                else
                {
                    menuItem.CurrentText = await UtilsManager.GetResource(Convert.ToInt64(menuItem.RowidResource));
                }

                if (menuItem.SubMenus != null && menuItem.SubMenus.Count > 0)
                {
                    await GetMenuResources(menuItem.SubMenus);
                }
            }
            NotifyMenuLoaded();
        }

        protected void NotifyMenuLoaded()
        {
            try
            {
                _menuLoaded?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}