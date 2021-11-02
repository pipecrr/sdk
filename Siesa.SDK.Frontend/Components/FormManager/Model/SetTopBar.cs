using Microsoft.AspNetCore.Components;
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
            if (Layout != null)
            {
                Layout.TopBarSetter = null;
            }
        }

    }
}
