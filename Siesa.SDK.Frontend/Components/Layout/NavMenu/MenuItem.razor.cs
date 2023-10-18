using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Siesa.SDK.Entities;


namespace Siesa.SDK.Frontend.Components.Layout.NavMenu;

public partial class MenuItem : SDKComponent
{
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Parameter] public E00061_Menu Menu { get; set; }
    [Parameter] public Dictionary<string, object> MenuItemAttributes { get; set; } = new Dictionary<string, object>();
    [Parameter] public string Title { get; set; } = "";
    [Parameter] public Action<E00061_Menu> OnClick { get; set; }
    [Parameter] public bool IsSuite { get; set; }
    [CascadingParameter] public Action HideSubmenu { get; set; }

    private bool ShowIconExternal { get; set; }


    private void MouseOver(MouseEventArgs e)
    { 
        ShowIconExternal=true; 
    }
    private void MouseOut(MouseEventArgs e)
    { 
        ShowIconExternal=false; 
    }

    private void OnClickItem(E00061_Menu menuItem, bool newTab = false)
    {
        if(menuItem != null && menuItem.CustomAction != null){
            menuItem.CustomAction.Invoke(menuItem);
            return;
        }
        if(newTab)
        {
            try
            {
               JSRuntime.InvokeAsync<object>("open", menuItem.CurrentURL, "_blank");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }else
        {
            NavigationManager.NavigateTo(menuItem.CurrentURL);
        }

        HideSubmenu?.Invoke();
    }

    protected override string GetAutomationId()
    {
        if(!string.IsNullOrEmpty(AutomationId))
        {
            if (Menu != null && Menu.Level > 1 && OnClick.Target != null)
            {
                var ParentMenu =  OnClick.Target.GetType().GetProperty("MainItem").GetValue(OnClick.Target);
                if (ParentMenu != null)
                {
                    var ParentMenuName = ParentMenu.GetType().GetProperty("CurrentText").GetValue(ParentMenu);
                    if (!string.IsNullOrEmpty(ParentMenuName.ToString()))
                    {
                        AutomationId = $"{ParentMenuName}_SubMenu_{Title}";
                    }
                }
            }
        }
        return base.GetAutomationId();
    }

    private string GetIconClass()
    {
        string MenuIcon = "";

        if(!string.IsNullOrEmpty(Menu.IconClass))
        {
            MenuIcon = Menu.IconClass; 
        }

        return MenuIcon;
    }

    private string GetStyleColor(){

        if(!string.IsNullOrEmpty(Menu.StyleColor))
        {
            return $"#{Menu.StyleColor};";
        }
        return "";
    }
    

}
