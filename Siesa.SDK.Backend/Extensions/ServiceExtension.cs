using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Backend.Interceptors;
using Siesa.SDK.Backend.Services;
using Siesa.SDK.Shared.Backend;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Criptography;
using Siesa.SDK.Shared.Logs.DataEventLog;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.DTOS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Siesa.SDK.Shared.Configurations;
using Siesa.SDK.Shared.Application;
using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
namespace Siesa.SDK.Backend.Extensions
{

    public static class ServiceExtensionExtension
    {
        public static void AddSDKBackend(this IServiceCollection services, ConfigurationManager configurationManager, Type ContextType)
        {
            var connectionConfig = configurationManager.GetSection("ConnectionConfig").Get<SDKConnectionConfig>();
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
            });

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });

            var dbConnections = configurationManager.GetSection("DbConnections").Get<List<SDKDbConnection>>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<MemoryService>();
            if(connectionConfig != null){
                services.AddScoped<ITenantProvider>( sp => ActivatorUtilities.CreateInstance<TenantProvider>(sp, dbConnections, connectionConfig));
            }else{
                services.AddScoped<ITenantProvider>( sp => ActivatorUtilities.CreateInstance<TenantProvider>(sp, dbConnections));
            }
            
            services.AddSingleton<IFeaturePermissionService, FeaturePermissionService>();
            services.AddSingleton<IBackendRouterService, BackendRouterService>();
            services.AddScoped<EmailService>();
            
            services.AddSingleton<IResourceManager, ResourceManager>(sp => ActivatorUtilities.CreateInstance<ResourceManager>(sp, false));
            services.AddScoped<ISDKJWT, Siesa.SDK.Backend.Criptography.SDKJWT>();
            
            services.AddSingleton<IQueueService, QueueService>(sp => ActivatorUtilities.CreateInstance<QueueService>(sp));
            services.AddHostedService<QueueService>();

            Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptionsAction = async (sp, opts) => 
            {
                var tenantProvider = sp.GetRequiredService<ITenantProvider>();
                var tenant = await tenantProvider.GetTenant();
                if(tenant == null){
                    //set first tenant as default
                    if(dbConnections.Count > 0){
                        tenantProvider.SetTenant(dbConnections[0]);
                        tenant = dbConnections[0];
                    }else{
                        throw new Exception("No tenant configured");
                    }
                }
                if(tenantProvider.GetUseLazyLoadingProxies()){
                    opts.UseLazyLoadingProxies();
                }
                if (tenant.ProviderName == EnumDBType.InMemory)
                {
                    //Pass
                }
                else if(tenant.ProviderName == EnumDBType.PostgreSQL)
                {
                    opts.UseNpgsql(tenantProvider.GetConnectionString(tenant));
                }else { //Default to SQL Server
                    opts.UseSqlServer(tenantProvider.GetConnectionString(tenant));
                }
                opts.AddInterceptors(new SDKDBInterceptor());
                opts.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.LazyLoadOnDisposedContextWarning));
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

            services.AddTransient(typeof(SDKContext), p => {
                var typeIDbContextFactory = typeof(IDbContextFactory<>).MakeGenericType(ContextType);
                dynamic factory = p.GetRequiredService(typeIDbContextFactory);
                return factory.CreateDbContext();
            });
            
            var awsOptions = new AWSOptions();
            var awsOptionsApp = configurationManager.GetSection("AWS").Get<SDKAWSOptionsDTO>();
            if(awsOptionsApp != null){
                awsOptions.Credentials = new BasicAWSCredentials(awsOptionsApp.AccessKeyId, awsOptionsApp.SecretAccessKey);
                awsOptions.Region = Amazon.RegionEndpoint.GetBySystemName(awsOptionsApp.Region);
                services.AddDefaultAWSOptions(awsOptions);
            }
            services.AddAWSService<IAmazonS3>();

        }
    }
}
