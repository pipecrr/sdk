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
namespace Siesa.SDK.Frontend {
    public static class SiesaSecurityExtensions
    {
        public static void AddSiesaSDKFrontend(this IServiceCollection services, IConfiguration serviceConfiguration)
        {
            services.AddBlazoredLocalStorage();
            services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);  // local storage
            services.AddDevExpressBlazor();
            services.AddScoped<StateContainer>();
            services.AddScoped<ILayoutService, LayoutService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<DialogService>();
            services.AddBlazorDragDrop();
        }

    }
}
