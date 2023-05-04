using Microsoft.AspNetCore.Components;

namespace Siesa.SDK.Frontend.Components.Dashboard {
    public enum SDKDashletWidth {
        Unknown,
        Full,
        Half,
        Third,
        Quarter
    }
    public class SDKDashletModel {
        public string Title { get; set; }
        public string Description { get; set; }
        public RenderFragment Content { get; set; }
        public string Icon { get; set; }
        public SDKDashletWidth Width { get; set; }
    }
}