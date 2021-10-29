using System;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Business;
using Siesa.SDK.Frontend.Components.FormManager.ViewModels;

namespace Siesa.SDK.Frontend.Components.FormManager.Views
{
    public partial class DynamicListView : DynamicBaseViewModel
    {
        public new RenderFragment CreateDynamicComponent() => builder =>
        {
            var viewType = typeof(Views.ListView);
            builder.OpenComponent(0, viewType);
            builder.AddAttribute(1, "BusinessObj", BusinessObj);
            builder.AddAttribute(2, "BusinessName", BusinessName);
            builder.CloseComponent();
        };
    }
}