using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.Backend;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Logs.DataEventLog;
using Siesa.SDK.Shared.Services;
using System;

namespace Siesa.SDK.Shared.Configurations
{
    public class ServiceConfiguration: IServiceConfiguration
    {
        public string MasterBackendUrl { get; set; } = String.Empty;
        public string AuditServerUrl { get; set; } = String.Empty;

        public string CurrentUrl { get; set; } = String.Empty;

    }

    public static class ServiceConfigurationExtension
    {
        public static void AddSDKServices(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            var serviceConfiguration = configurationManager.GetSection("ServiceConfiguration");
            ServiceConfiguration sc = serviceConfiguration.Get<ServiceConfiguration>();
            services.Configure<ServiceConfiguration>(serviceConfiguration);
            services.AddScoped<IServiceConfiguration, ServiceConfiguration>();
            services.AddOptions();
            services.AddLogging(builder => SDKLogger.Configure(builder, sc));
        }
    }
}
