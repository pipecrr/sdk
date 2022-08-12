using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.Layout;
using Microsoft.Extensions.Configuration;
using Siesa.SDK.Shared.Configurations;
using Siesa.SDK.Shared.Backend;
using Blazored.LocalStorage;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Frontend.Services;
using Radzen;
using Plk.Blazor.DragDrop;
using Siesa.SDK.Frontend.Application;
using Siesa.SDK.Shared.Criptography;

namespace Siesa.SDK.Frontend {
    public static class SiesaSecurityExtensions
    {
        public static void AddSiesaSDKFrontend(this IServiceCollection services, IConfiguration serviceConfiguration)
        {
            services.AddBlazoredLocalStorage();
            services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);  // local storage
            services.AddDevExpressBlazor();
            services.AddScoped<ILayoutService, LayoutService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<DialogService>();
            services.AddScoped<NavigationService>();
            services.AddBlazorDragDrop();
            services.AddSingleton<IResourceManager, ResourceManager>();
            services.AddSingleton<IFeaturePermissionService, FeaturePermissionService>();
            services.AddScoped<UtilsManager>(sp => ActivatorUtilities.CreateInstance<UtilsManager>(sp));
            services.AddScoped<NotificationService, SDKNotificationService>(sp => ActivatorUtilities.CreateInstance<SDKNotificationService>(sp));
            services.AddScoped<SDKNotificationService>(sp => (SDKNotificationService)sp.GetRequiredService<NotificationService>());

            services.AddScoped<ISDKJWT, Siesa.SDK.Frontend.Criptography.SDKJWT>();
        }

    }
}
