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
using Siesa.Global.Enums;
using Siesa.SDK.Shared.Business;

namespace Siesa.SDK.Frontend.Services
{
    public class MenuService
    {
        public List<E00061_Menu> Menus { get; set; }
        public List<E00060_Suite> Suites { get; set; }
        private IBackendRouterService BackendRouterService;
        private IAuthenticationService AuthenticationService;
        private UtilsManager UtilsManager { get; set; }
        public Dictionary<int, List<E00061_Menu>> SuiteData {get; set;} = new();

        public E00060_Suite SelectedSuite { get; set; } = new();

        private bool loading = false;

        private SDKBusinessModel _menuBL { get { return BackendRouterService.GetSDKBusinessModel("BLAdminMenu", AuthenticationService); } }

        public MenuService(IBackendRouterService backendRouterService, IAuthenticationService authenticationService, UtilsManager utilsManager)
        {
            BackendRouterService = backendRouterService;
            AuthenticationService = authenticationService;
            UtilsManager = utilsManager;
            Menus = new List<E00061_Menu>();

            //_ = Init();
        }

        private async Task GetSuites()
        {
            Suites = new List<E00060_Suite>();
            var request = await _menuBL.Call("GetSuites");
            if (request.Success)
            {
                Suites = (List<E00060_Suite>)request.Data;
            }

            if(Suites.Count == 1 || AuthenticationService.GetSelectedSuite() == 0)
            {
                AuthenticationService.SetSelectedSuite(Suites.First().Rowid);
                SelectedSuite = Suites.FirstOrDefault(x => x.Rowid == Suites.First().Rowid);
            }
            else
            {
                SelectedSuite = Suites.FirstOrDefault(x => x.Rowid == AuthenticationService.GetSelectedSuite());
            }

            _ = GetSuiteResources(Suites);
        }
        private async Task GetSuiteResources(List<E00060_Suite> _suites)
        {
            foreach (var Suite in _suites)
            {
                if (string.IsNullOrEmpty(Suite.CurrentText) && Suite.RowidResource != 0)
                {
                  Suite.CurrentText = await UtilsManager.GetResource(Suite.RowidResource);
                }                
            }
        }

        private void AddDevMenu(List<E00061_Menu> menus)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == Environments.Development)
            {
                Menus.Add(new E00061_Menu()
                {
                    ResourceTag = "SDKDev-DevMenu",
                    IconClass = "fa-solid fa-code",
                    SubMenus = new List<E00061_Menu>(),
                    CurrentText = "DevMenu",
                });
                var DevMenu = Menus.FirstOrDefault(x => x.ResourceTag == "SDKDev-DevMenu")?.SubMenus;

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
                            ResourceTag = $"SDKDev-{business.Name}.Plural",
                            Url = $"/{business.Name}/explorer/",
                            Type = MenuType.CustomMenu
                        };
                        DevMenu.Add(customActionMenu);
                    }
                    else
                    {
                        var submenuItem = new E00061_Menu
                        {
                            ResourceTag = $"SDKDev-{business.Name}.Plural",
                            Url = $"/{business.Name}/",
                            SubMenus = new List<E00061_Menu>(),
                            Type = MenuType.CustomMenu
                        };
                        //search methods that return a RenderFragment
                        var customActions = businessType.GetMethods().Where(m => m.ReturnType == typeof(RenderFragment));
                        foreach (var customAction in customActions)
                        {
                            var customActionMenu = new E00061_Menu
                            {
                                ResourceTag = $"SDKDev-{business.Name}.CustomAction.{customAction.Name}",
                                Url = $"/{business.Name}/{customAction.Name}/",
                                Type = MenuType.CustomMenu
                            };
                            submenuItem.SubMenus.Add(customActionMenu);
                        }

                        DevMenu.Add(submenuItem);
                    }
                }
            }
        }

        private async Task Init()
        {
            if(loading)
            {
                return;
            }
            await GetSuites();

            loading = true;
            Menus = new List<E00061_Menu>();
            AddDevMenu(Menus);
            int rowidSuite = AuthenticationService.GetSelectedSuite(); 
            await LoadMenu(rowidSuite);
            loading = false;
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

        public async Task ReloadMenu()
        {
            if(Menus != null)
            {
                Menus.Clear();
            }
            await Init();
        }

        public void ClearMenu()
        {
            Menus.Clear();
        }

        public async Task LoadMenu(int suiteRowid)
        {
            var menuRequest = await _menuBL.Call("GetMenuItems", Convert.ToInt64(suiteRowid));

            if(menuRequest.Success)
            {
                var Data = menuRequest.Data;
                MenuManagerBySuite(_menuBL, suiteRowid, Data);
            }
        }

        public async Task<bool> GetMenuItemsWithoutSubLevelsBySuite(SDKBusinessModel menuBL, int RowidSuite)
        {
            if(menuBL is null || RowidSuite <= 0) return false;

            var menuRequest = await menuBL.Call("GetMenuItemsWithoutSubLevels", RowidSuite, 0);

            if(!menuRequest.Success) return false;

            var Data = menuRequest.Data;
            MenuManagerBySuite(menuBL, RowidSuite, Data, true, true);

            return true;
        }

        public async Task<List<E00061_Menu>> GetMenuItemsWithoutSubLevelsByMenu(SDKBusinessModel menuBL, int RowidMenuParent)
        {
            if(menuBL is null || RowidMenuParent <= 0) return null;

            var menuRequest = await menuBL.Call("GetMenuItemsWithoutSubLevels", 0, RowidMenuParent);

            if(!menuRequest.Success) return null;

            var Data = menuRequest.Data;
            MenuManagerBySuite(menuBL, 0, Data, true, false);

            return Data;
        }

        public async Task<bool> GetMenuItemsWithChilds(int RowidSuite)
        {
            var menuBL = BackendRouterService.GetSDKBusinessModel("BLAdminMenu", AuthenticationService);

            var Request = await menuBL.Call("GetMenuItemsWithChilds", RowidSuite);

            if(!Request.Success) return false;

            var Data = Request.Data;

            MenuManagerBySuite(menuBL, RowidSuite, Data, true, true);

            return true;
        }

        public async Task<bool> GetMenuItemsWithChildsAndOperations(int RowidSuite)
        {
            var menuBL = BackendRouterService.GetSDKBusinessModel("BLAdminMenu", AuthenticationService);

            var Request = await menuBL.Call("GetMenuItemsWithChildsAndOperations", RowidSuite);

            if(!Request.Success) return false;

            var Data = Request.Data;

            MenuManagerBySuite(menuBL, RowidSuite, Data, true, true);

            return true;
        }

        public void MenuManagerBySuite(SDKBusinessModel menuBL, int RowidSuite, List<E00061_Menu> menuResponse, bool IgnoreGeneralMenu = false, bool SetInSuiteData = false)
        {
            menuResponse = menuResponse.OrderBy(x => x.Order).ToList();

            if(!IgnoreGeneralMenu)
            {
                //add menuResponse to Menus
                Menus.AddRange(menuResponse);
            }else
            {
                if(SetInSuiteData && !SuiteData.ContainsKey(RowidSuite))
                {
                    SuiteData.Add(RowidSuite, menuResponse);
                }
            }

            _ = GetMenuResources(menuResponse);
            NotifyMenuLoaded();
        }


        private async Task GetMenuResources(ICollection<E00061_Menu> _menus)
        {
            foreach (var menuItem in _menus)
            {
                if(menuItem.Type == MenuType.Separator)
                {
                    menuItem.CurrentText = " ";
                    continue;
                }
                if(menuItem.Feature != null && (menuItem.RowidResource == null || menuItem.RowidResource == 0) && menuItem.ResourceTag == null)
                {
                    menuItem.ResourceTag = $"{menuItem.Feature.BusinessName}.Plural";
                }

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

        public async Task SetSelectedSuite(int suiteRowid)
        {
            var suite = Suites.FirstOrDefault(x => x.Rowid == suiteRowid);
            if (suite != null)
            {
                SelectedSuite = suite;
                AuthenticationService.SetSelectedSuite(suiteRowid);
                Menus.Clear();
                AddDevMenu(Menus);
                if (SuiteData.ContainsKey(suiteRowid))
                {
                    Menus.AddRange(SuiteData[suiteRowid]);
                }
                else
                {
                    await LoadMenu(suiteRowid);
                }
            }else
            {
                throw new Exception("Suite not found");
            }
        }
    }
}