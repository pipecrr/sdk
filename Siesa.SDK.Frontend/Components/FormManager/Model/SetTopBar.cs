using Microsoft.AspNetCore.Components;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Components.FormManager.Views;
using Siesa.SDK.Frontend.Components.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Components.FormManager.Model
{
    public class SetTopBar : ComponentBase, IDisposable
    {
        [Inject]
        private ILayoutService Layout { get; set; }

        [Parameter]
        public RenderFragment TopBarTitle { get; set; }

        [Parameter]
        public RenderFragment TopBarButtons { get; set; }

        [Parameter]
        public RenderFragment TopBarExtraButtons { get; set; }

        [Parameter] 
        public string StyleName { get; set; } = "toolbar_default";

        [Parameter] 
        public bool HasExtraButtons { get; set; }

        [Parameter]	
        public dynamic BusinessObj { get; set; }

        [Parameter]
        public bool HiddenCompaies { get; set; } = false;

        [Parameter]
        public bool DisableCompanies { get; set; }

        [Parameter] 
        public EventCallback<E00201_Company> OnChangeCompany { get; set; }

        [Parameter]
        public ListView ListView { get; set; }



        protected override void OnInitialized()
        {
            if (Layout != null)
            {
                Layout.TopBarSetter = this;
            }
            base.OnInitialized();
        }

        protected override bool ShouldRender()
        {
            var shouldRender = base.ShouldRender();
            if (shouldRender)
            {
                Layout.UpdateTopBar();
            }
            return shouldRender;
        }

        public void Dispose()
        {
            if (Layout != null && Layout.TopBarSetter == this)
            {
                Layout.TopBarSetter.TopBarExtraButtons = null;
                Layout.TopBarSetter.TopBarButtons = null;
                Layout.TopBarSetter = null;
            }
        }

    }
}
