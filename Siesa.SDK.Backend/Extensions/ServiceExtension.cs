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
            services.AddSingleton<IBackendRouterService, BackendRouterService>();
            services.AddScoped<EmailService>();
            services.AddSingleton<IResourceManager, ResourceManager>(sp => ActivatorUtilities.CreateInstance<ResourceManager>(sp, false));
            services.AddSingleton<QueueService>();
            services.AddScoped<ISDKJWT, Siesa.SDK.Backend.Criptography.SDKJWT>();

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
                if(tenantProvider.GetUseLazyLoadingProxies()){
                    opts.UseLazyLoadingProxies();
                }
                if (tenant.ProviderName == EnumDBType.InMemory)
                {
                    //Pass
                }
                else if(tenant.ProviderName == EnumDBType.PostgreSQL)
                {
                    opts.UseNpgsql(tenant.ConnectionString);
                }else { //Default to SQL Server
                    //add Application Name= if not present
                    if(!tenant.ConnectionString.Contains("Application Name=")){
                        var serviceConfiguration = configurationManager.GetSection("ServiceConfiguration");
                        ServiceConfiguration sc = serviceConfiguration.Get<ServiceConfiguration>();
                        var currentUrl = sc?.GetCurrentUrl();
                        if(!string.IsNullOrEmpty(currentUrl)){
                            //delete http:// or https://
                            currentUrl = currentUrl.Replace("http://", "");
                            currentUrl = currentUrl.Replace("https://", "");
                            //delete last /
                            if(currentUrl.Last() == '/'){
                                currentUrl = currentUrl.Substring(0, currentUrl.Length - 1);
                            }
                        }
                        //get machine name
                        var machineName = Environment.MachineName;
                        //check if last char is ;
                        if(tenant.ConnectionString.Last() != ';'){
                            tenant.ConnectionString += ";";
                        }
                        var appName = $"{machineName}";

                        if(!string.IsNullOrEmpty(currentUrl)){
                            appName += $"-{currentUrl}";
                        }
                        tenant.ConnectionString += $"Application Name=SDK-{appName};";
                    }
                    opts.UseSqlServer(tenant.ConnectionString);

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
