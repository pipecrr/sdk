using Microsoft.AspNetCore.Components;

namespace Siesa.SDK.Frontend.Components
{
    public abstract class SDKComponent : ComponentBase
    {
        [Parameter]
        public string AutomationId { get; set; }

        protected virtual string GetAutomationId()
        {
            return $"{this.GetType().Name}_{AutomationId}";
        }
    }
}