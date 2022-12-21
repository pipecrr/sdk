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


using WebDesigner_Blazor.Implementation;
using System.IO;
using WebDesigner_Blazor.Services;

namespace Siesa.SDK.Frontend {
    public static class SiesaSecurityExtensions
    {
        // private static readonly DirectoryInfo ResourcesRootDirectory =
		// 	new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "resources" + Path.DirectorySeparatorChar));

		// private static readonly DirectoryInfo TemplatesRootDirectory =
		// 	new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "templates" + Path.DirectorySeparatorChar));

		// private static readonly DirectoryInfo DataSetsRootDirectory =
		// 	new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "datasets" + Path.DirectorySeparatorChar));
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
            services.AddHttpContextAccessor();


            services.AddReporting();
			services.AddDesigner();
			// services.AddSingleton<ITemplatesService>(new FileSystemTemplates(TemplatesRootDirectory));
			// services.AddSingleton<IDataSetsService>(new FileSystemDataSets(DataSetsRootDirectory));
			// services.AddRazorPages().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
			// services.AddServerSideBlazor();
           /* services.AddBlazorDB(options =>
            {
                options.Name = "SiesaSDK";
                options.Version = 1;
                options.StoreSchemas = new List<StoreSchema>() {
                     new StoreSchema()
                    {
                        Name = "Person",
                        PrimaryKey = "id",
                        PrimaryKeyAuto = true,
                        UniqueIndexes = new List<string> { "guid" },
                        Indexes = new List<string> { "name" }
                    }
                };
            });*/

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
