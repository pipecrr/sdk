using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Backend.Interceptors;
using Siesa.SDK.Backend.Services;
using Siesa.SDK.Shared.Backend;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Logs.DataEventLog;
using Siesa.SDK.Shared.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Siesa.SDK.Backend.Extensions
{

    public static class ServiceExtensionExtension
    {
        public static void AddSDKBackend(this IServiceCollection services, ConfigurationManager configurationManager, Type ContextType)
        {

            var dbConnections = configurationManager.GetSection("DbConnections").Get<List<SDKDbConnection>>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<ITenantProvider>( sp => ActivatorUtilities.CreateInstance<TenantProvider>(sp, dbConnections));
            services.AddSingleton<IFeaturePermissionService, FeaturePermissionService>();

            Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptionsAction = (sp, opts) =>
            {
                var tenantProvider = sp.GetRequiredService<ITenantProvider>();
                var tenant = tenantProvider.GetTenant();
                if(tenant == null){
                    //set first tenant as default
                    if(dbConnections.Count > 0){
                        tenantProvider.SetTenant(dbConnections[0]);
                        tenant = dbConnections[0];
                    }else{
                        throw new Exception("No tenant configured");
                    }
                }
                //opts.UseLazyLoadingProxies(); //TODO: Habilitar cuando corrijan el bug en efcore (no se desactiva con IDbContextFactory)
                if(tenant.ProviderName == EnumDBType.PostgreSQL)
                {
                    opts.UseNpgsql(tenant.ConnectionString);
                }else { //Default to SQL Server
                    opts.UseSqlServer(tenant.ConnectionString);

                }
                opts.AddInterceptors(new SDKDBInterceptor());
            };
            var typeExt = typeof (Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions);
            var method = typeExt.GetMethods().Single( x => {
                var p = x.GetParameters();
                var g = x.GetGenericArguments();
                return x.Name == "AddDbContextFactory" && g.Length == 1 && p.Length == 3 && p[1].ParameterType == typeof(Action<IServiceProvider, DbContextOptionsBuilder>);
            });
            var generic = method.MakeGenericMethod(ContextType);
            generic.Invoke(services, new object[] { services, dbContextOptionsAction, ServiceLifetime.Transient });
            //builder.Services.AddScoped(typeof(IDbContextFactory<SDKContext>), p => p.GetRequiredService<IDbContextFactory<ProjectContext>>());
            services.AddScoped(typeof(IDbContextFactory<SDKContext>), p => {
                var typeIDbContextFactory = typeof(IDbContextFactory<>).MakeGenericType(ContextType);
                var factory = p.GetRequiredService(typeIDbContextFactory);
                return factory;
            });

        }
    }
}
