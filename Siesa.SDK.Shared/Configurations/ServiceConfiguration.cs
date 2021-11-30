using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Shared.Logs.DataEventLog;

namespace Siesa.SDK.Shared.Configurations
{
    public static class ServiceConfiguration
    {
        public static void AddSDKServices(this IServiceCollection services)
        {            
            services.AddLogging(builder => SDKLogger.Configure(builder));
        }
    }
}
