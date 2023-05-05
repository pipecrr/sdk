using Microsoft.AspNetCore.Components;

namespace Siesa.SDK.Frontend.Components.Dashboard {
    public abstract class SDKDashlet : ComponentBase {
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string Icon { get; set; }
        public virtual SDKDashletWidth Width { get; set; }
    }
}