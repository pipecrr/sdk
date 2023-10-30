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
using System.Collections.Generic;
using GrapeCity.ActiveReports.Aspnetcore.Designer;
using GrapeCity.ActiveReports.Aspnetcore.Viewer;
using System.IO;

using Siesa.SDK.Report.Implementation;
using Siesa.SDK.Frontend.Report.Controllers;
using GrapeCity.ActiveReports.Aspnetcore.Designer.Services;
using Siesa.SDK.Frontend.Report.Services;
using Microsoft.AspNetCore.ResponseCompression;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Blazored.Toast;

namespace Siesa.SDK.Frontend
{
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
            services.AddSingleton<IViewdefManager, ViewdefManager>();
            services.AddSingleton<IFeaturePermissionService, FeaturePermissionService>();
            services.AddScoped<UtilsManager>(sp => ActivatorUtilities.CreateInstance<UtilsManager>(sp));
            services.AddScoped<SaveController>(sp => ActivatorUtilities.CreateInstance<SaveController>(sp));
            services.AddScoped<NotificationService, SDKNotificationService>(sp => ActivatorUtilities.CreateInstance<SDKNotificationService>(sp));
            services.AddScoped<SDKNotificationService>(sp => (SDKNotificationService)sp.GetRequiredService<NotificationService>());
            services.AddScoped<ISDKJWT, Siesa.SDK.Frontend.Criptography.SDKJWT>();
            services.AddScoped<SDKDialogService>();
            services.AddScoped<SDKGlobalLoaderService>();
            services.AddScoped<MenuService>();
            services.AddScoped<IQueueService, QueueService>();
            services.AddBlazoredToast();
            services.AddSignalR(e =>
            {
                e.MaximumReceiveMessageSize = 102400000;
            });
            services.AddHttpContextAccessor();

            services.AddReporting();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment != Environments.Development)
            {
                services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = System.IO.Compression.CompressionLevel.Optimal;
                });
                services.AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/javascript" });
                    options.Providers.Add<GzipCompressionProvider>();
                });
            }
            services.AddDesigner();
            services.AddSingleton<ITemplatesService>(new SDKSystemTemplates());
            services.AddRazorPages().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddServerSideBlazor();

            services.AddScoped<IIndexedDbFactory, IndexedDbFactory>();

            SDKApp.AddAssembly(typeof(LayoutService).Assembly);
        }

        public static IApplicationBuilder UseSDK(this IApplicationBuilder app)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment != Environments.Development)
            {
                app.UseResponseCompression();
            }
            app.UseHttpsRedirection();


            app.UseStaticFiles();

            var serviceScope = app.ApplicationServices.CreateScope().ServiceProvider;

            var resourcesSaveService = serviceScope.GetRequiredService<SaveController>();
            app.UseReporting(config =>
            {
                config.UseCustomStore(id =>
                {
                    return resourcesSaveService.GetReport(id);
                });
                GrapeCity.ActiveReports.Web.Viewer.DataProviderInfo[] providers = new[]{
                    new GrapeCity.ActiveReports.Web.Viewer.DataProviderInfo("Siesa.SDK.Business", typeof(SDKReportProvider).AssemblyQualifiedName),
                };
                config.UseDataProviders(providers);

            });

            app.UseDesigner(config =>
            {
                config.UseCustomStore(resourcesSaveService);
                DataProviderInfo[] providers = new[]{
                    new DataProviderInfo("Siesa.SDK.Business", typeof(SDKReportProvider).AssemblyQualifiedName),
                };

                config.UseDataProviders(providers);
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/login",
                    defaults: new { controller = "Api", action = "GetSessionToken" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{blname}/{blaction}/",
                    defaults: new { controller = "Api", action = "Index" });
                //sdk-docs/* redirect
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "sdk-docs/{*url}",
                    defaults: new { controller = "Api", action = "RedirectToDocs" });
            });


            return app;
        }

    }
}
