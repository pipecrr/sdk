using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.Backend;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Logs.DataEventLog;
using System;

namespace Siesa.SDK.Shared.Configurations
{
    public class ServiceConfiguration: IServiceConfiguration
    {
        public string MasterBackendUrl { get; set; } = String.Empty;

    }

    public static class ServiceConfigurationExtension
    {
        public static void AddSDKServices(this IServiceCollection services, IConfiguration serviceConfiguration)
        {            
            services.AddLogging(builder => SDKLogger.Configure(builder));
            services.Configure<ServiceConfiguration>(serviceConfiguration);
            services.AddOptions();
            ServiceConfiguration sc = serviceConfiguration.Get<ServiceConfiguration>();
            BackendManager.SetMasterBackendUrl(sc.MasterBackendUrl);
            
        }
    }
}
