using System;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;
using Siesa.SDK.Frontend.Components.FormManager.Views;

namespace Siesa.SDK.Frontend.Components.FormManager
{
    public static class FormUtils
    {
        public static RenderFragment RenderFreeForm(string viewdef, dynamic BusinessObj, string Title, DynamicViewType ViewContext = DynamicViewType.Create, bool SetTopBar = true, string key = "")
        {
            if(string.IsNullOrEmpty(key)) 
            {
                key = Guid.NewGuid().ToString();
            }
            return (builder) =>
            {
                builder.OpenComponent<FreeForm>(0);
                builder.AddAttribute(1, "Viewdef", viewdef);
                builder.AddAttribute(2, "BusinessObj", BusinessObj);
                builder.AddAttribute(3, "BusinessName", BusinessObj.BusinessName);
                builder.AddAttribute(4, "Title", Title);
                builder.AddAttribute(5, "ViewContext", ViewContext);
                builder.AddAttribute(6, "SetTopBar", SetTopBar);
                builder.SetKey(key);
                builder.CloseComponent();
            };
        }
    }


}