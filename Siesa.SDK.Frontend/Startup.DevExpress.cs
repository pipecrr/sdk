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
using Siesa.SDK.Shared.Application;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Builder;
using Blazor.IndexedDB.Framework;
namespace Siesa.SDK.Frontend {
    public static class SiesaSecurityExtensions
    {
        public static void AddSiesaSDKFrontend(this IServiceCollection services, IConfiguration serviceConfiguration)
        {
            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(SiesaSecurityExtensions).Assembly));
            services.AddBlazoredLocalStorage();
            services.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);  // local storage
            services.AddDevExpressBlazor();
            services.AddScoped<ILayoutService, LayoutService>();
            services.AddSingleton<IBackendRouterService, BackendRouterService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<DialogService>();
            services.AddScoped<NavigationService>();
            services.AddBlazorDragDrop();
            services.AddSingleton<IResourceManager, ResourceManager>();
            services.AddScoped<IFeaturePermissionService, FeaturePermissionService>();
            services.AddScoped<UtilsManager>(sp => ActivatorUtilities.CreateInstance<UtilsManager>(sp));
            services.AddScoped<NotificationService, SDKNotificationService>(sp => ActivatorUtilities.CreateInstance<SDKNotificationService>(sp));
            services.AddScoped<SDKNotificationService>(sp => (SDKNotificationService)sp.GetRequiredService<NotificationService>());
            

            services.AddScoped<ISDKJWT, Siesa.SDK.Frontend.Criptography.SDKJWT>();
            services.AddScoped<SDKDialogService>();
            services.AddScoped<MenuService>();
            services.AddSignalR(e => {
                e.MaximumReceiveMessageSize = 102400000;
            });
            
            services.AddScoped<IIndexedDbFactory,   IndexedDbFactory>();

            SDKApp.AddAssembly(typeof(LayoutService).Assembly);
        }

        public static IApplicationBuilder UseSDK(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{blname}/{blaction}/",
                    defaults: new { controller = "Api", action = "Index" });
            });
            return app;
        }

    }
}
