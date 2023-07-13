

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Siesa.SDK.Frontend.Components.Documentation.Playground;
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

        public Type ComponentType { get; set; }

        public string SourceCode { get; set; }
    }

    public partial class SDKDocumentation : ComponentBase
    {
        private string _codeViewerId = Guid.NewGuid().ToString();
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
            _codeViewerId = Guid.NewGuid().ToString();
            StateHasChanged();
        }

        private void Init()
        {
            if (!string.IsNullOrEmpty(pComponentName))
            {
                if (pComponentName == "playground")
                {
                    SelectedComponent = new ComponentDemo()
                    {
                        ComponentName = "playground",
                        ComponentType = typeof(PlaygroundView)
                    };
                }
                else
                {
                    var x = Category.Where(x => x.Components.Any(y => y.ComponentName == pComponentName)).FirstOrDefault();
                    if (x != null)
                    {
                        SelectedComponent = x.Components.Where(x => x.ComponentName == pComponentName).FirstOrDefault();
                    }
                }
                if (SelectedComponent != null)
                {
                    _codeViewerId = Guid.NewGuid().ToString();
                    StateHasChanged();
                }
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Init();
        }

        public string ViewSourceCode(ComponentDemo item)
        {
            try
            {
                string source = Utilities.ReadAssemblyResource(this.GetType().Assembly, $"Components.Documentation.SourceCode.{item.ComponentType.Name}.txt");
                return item.SourceCode = source;
                
            }
            catch (Exception)
            {
                _ = Notification.ShowError("Custom.SDKDocumentation.SourceCodeNotFound");
                StateHasChanged();
                return "";
            }

        }
    }

}