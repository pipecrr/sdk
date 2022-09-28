

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Shared.Utilities;

namespace Siesa.SDK.Frontend.Components.Documentation
{

    public class ComponentCategory
    {

        public string Name { get; set; }
        public List<ComponentDemo> Components { get; set; } = new List<ComponentDemo>();

        public string Icon { get; set; }

    }
    public class ComponentDemo
    {

        public string ComponentName { get; set; }

        public RenderFragment ComponentFragment { get; set; }

        public string SourceCode { get; set; }
    }

    public partial class SDKDocumentation : ComponentBase
    {
        private string GetNavigateUrl(ComponentDemo item)
        {
            return $"/sdk-docs/{item.ComponentName}";
        }
        [Parameter] public string pComponentName { get; set; }

        private ComponentDemo SelectedComponent { get; set; }
        [Inject] public SDKNotificationService Notification { get; set; }

        private bool showSource;

        public List<ComponentCategory> Category { get; set; } = new List<ComponentCategory>();


        private void SetSelectedComponent(ComponentDemo item)
        {
            SelectedComponent = item;
            StateHasChanged();
        }

        private void Init()
        {
            if (!string.IsNullOrEmpty(pComponentName))
            {
                var x = Category.Where(x => x.Components.Any(y => y.ComponentName == pComponentName)).FirstOrDefault();
                if (x != null)
                {
                    SelectedComponent = x.Components.Where(x => x.ComponentName == pComponentName).FirstOrDefault();
                }
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Init();
        }

        public void ViewSourceCode()
        {

            showSource = !showSource;
            if (showSource)
            {
                try
                {
                    string prubaURL = Utilities.ReadAssemblyResource(this.GetType().Assembly, $"Components.Documentation.SourceCode.{SelectedComponent.ComponentName}.txt");
                    SelectedComponent.SourceCode = prubaURL;
                }
                catch (Exception e)
                {

                    Notification.ShowError("Custom.SDKDocumentation.SourceCodeNotFound");
                    StateHasChanged();
                }

            }

        }
    }

}