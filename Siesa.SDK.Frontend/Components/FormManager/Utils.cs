using System;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.FormManager.Views;

namespace Siesa.SDK.Frontend.Components.FormManager
{
    public static class FormUtils
    {
        public static RenderFragment RenderFreeForm(string viewdef, dynamic BusinessObj, string Title)
        {
            return (builder) =>
            {
                builder.OpenComponent<FreeForm>(0);
                builder.AddAttribute(1, "Viewdef", viewdef);
                builder.AddAttribute(2, "BusinessObj", BusinessObj);
                builder.AddAttribute(3, "BusinessName", BusinessObj.BusinessName);
                builder.AddAttribute(4, "Title", Title);
                builder.CloseComponent();
            };
        }
    }


}