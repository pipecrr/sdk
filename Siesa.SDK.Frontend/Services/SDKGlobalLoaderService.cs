using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Siesa.SDK.Frontend.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.Visualization;

namespace Siesa.SDK.Frontend.Services
{
    public class SDKGlobalLoaderServices
    {
        private readonly SDKGlobalLoader _SDKGlobalLoader;
        public SDKGlobalLoaderServices(SDKGlobalLoader SDKGlobalLoader)
        {
            _SDKGlobalLoader = SDKGlobalLoader;
        }

        public RenderFragment<SDKGlobalLoader> GetLoader()
        {
            return (_SDKGlobalLoader) => builder =>
            {
                builder.OpenComponent(0, typeof(SDKGlobalLoader));
               //builder.OpenComponent<SDKGlobalLoader>(0);
                builder.CloseComponent();
            };
        }

    }
}