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

namespace Siesa.SDK.Frontend {
    public static class SiesaSecurityExtensions
    {
		// private static readonly DirectoryInfo TemplatesRootDirectory =
		// 	new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "templates" + Path.DirectorySeparatorChar));

		


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
            services.AddScoped<SaveController>(sp => ActivatorUtilities.CreateInstance<SaveController>(sp));
            services.AddScoped<NotificationService, SDKNotificationService>(sp => ActivatorUtilities.CreateInstance<SDKNotificationService>(sp));
            services.AddScoped<SDKNotificationService>(sp => (SDKNotificationService)sp.GetRequiredService<NotificationService>());
            

            services.AddScoped<ISDKJWT, Siesa.SDK.Frontend.Criptography.SDKJWT>();
            services.AddScoped<SDKDialogService>();
            services.AddScoped<SDKGlobalLoaderService>();
            services.AddScoped<MenuService>();
            services.AddSignalR(e => {
                e.MaximumReceiveMessageSize = 102400000;
            });
            services.AddHttpContextAccessor();


            services.AddReporting();
			services.AddDesigner();
			services.AddSingleton<ITemplatesService>(new SDKSystemTemplates());
			services.AddRazorPages().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
			services.AddServerSideBlazor();
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
            var serviceScope = app.ApplicationServices.CreateScope().ServiceProvider;

            var resourcesSaveService = serviceScope.GetRequiredService<SaveController>();
            app.UseReporting(config => {
                config.UseCustomStore(id => {
                    //SaveController _saveController = ActivatorUtilities.CreateInstance<SaveController>(serviceScope.ServiceProvider);
                    return resourcesSaveService.GetReport(id);
                });
                GrapeCity.ActiveReports.Web.Viewer.DataProviderInfo[] providers = new []{
                    new GrapeCity.ActiveReports.Web.Viewer.DataProviderInfo("Siesa.SDK.Business", typeof(SDKReportProvider).AssemblyQualifiedName),
                };
                config.UseDataProviders(providers);

            });

            app.UseDesigner(config => {
                //SaveController _saveController = ActivatorUtilities.CreateInstance<SaveController>(serviceScope.ServiceProvider);
                //config.EnabledDataApi = false;
                config.UseCustomStore(resourcesSaveService);
                DataProviderInfo[] providers = new []{
                    new DataProviderInfo("Siesa.SDK.Business", typeof(SDKReportProvider).AssemblyQualifiedName),
                };
                
                config.UseDataProviders(providers);
                // config


            });

            app.UseRouting();
            
            app.UseEndpoints(endpoints =>

            {
                endpoints.MapControllers();
            });
            return app;
        }

    }
}
