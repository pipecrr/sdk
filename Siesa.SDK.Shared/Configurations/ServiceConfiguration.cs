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

        private string _currentUrl = String.Empty;

        public string GetCurrentUrl()
        {
            if(!string.IsNullOrEmpty(_currentUrl))
            {
                return _currentUrl;
            }
            
            if (string.IsNullOrEmpty(CurrentUrl))
            {
                var machineName = Environment.MachineName;
                return _currentUrl = $"https://{machineName}";
            }
            else
            {
                return _currentUrl = CurrentUrl;
            }
        }

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
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            services.AddLogging(builder => SDKLogger.Configure(builder, serviceProvider));
        }
    }
}
