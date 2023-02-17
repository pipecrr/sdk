using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Siesa.SDK.Frontend.Components;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.Visualization;

namespace Siesa.SDK.Frontend.Services
{
    public class SDKGlobalLoaderService
    {
        private  SDKGlobalLoader _refSDKGlobalLoader;
        public SDKGlobalLoaderService()
        {
        }

        public void SetRef(SDKGlobalLoader refSDKGlobalLoader)
        {
            _refSDKGlobalLoader = refSDKGlobalLoader;
        }

        public void Show(string messageResourceTag = "")
        {
            _refSDKGlobalLoader.ShowLoader(messageResourceTag);
        }

        public void Hide()
        {
            _refSDKGlobalLoader.HideLoader();
        }

    }
}