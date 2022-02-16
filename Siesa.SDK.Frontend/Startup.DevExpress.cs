using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.Layout;
using Microsoft.Extensions.Configuration;
using Siesa.SDK.Shared.Configurations;
using Siesa.SDK.Shared.Backend;

namespace Siesa.SDK.Frontend {
    public static class SiesaSecurityExtensions
    {
        public static void AddSiesaSDKFrontend(this IServiceCollection services, IConfiguration serviceConfiguration)
        {
            services.AddDevExpressBlazor();
            services.AddScoped<StateContainer>();
            services.AddScoped<ILayoutService, LayoutService>();
            ServiceConfiguration sc = serviceConfiguration.Get<ServiceConfiguration>();
            BackendManager.SetMasterBackendUrl(sc.MasterBackendUrl);

            //TODO: Definir en donde se debe hacer esto
            BackendManager.Instance.SyncWithMasterBackend();
            foreach (var backend in BackendManager.Instance.GetBackendDict())
            {
                foreach (var business in backend.Value.businessRegisters.Businesses)
                {
                    BusinessManagerFrontend.Instance.AddBusiness(business, backend.Value.Name);
                }
            }
        }

    }
}
