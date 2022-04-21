using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Backend.Services;
using Siesa.SDK.Shared.Backend;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Logs.DataEventLog;
using Siesa.SDK.Shared.Services;
using System;

namespace Siesa.SDK.Backend.Extensions
{

    public static class ServiceExtensionExtension
    {
        public static void AddSiesaSDKBackend(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
        }
    }
}