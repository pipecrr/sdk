//------------------------------------------------------------------------------
//<auto-generated>
// Generated by the DevExpress.Blazor package.
// To prevent this operation, add the DxExtendStartupHost property to the project and set this property to False.
//
// Siesa.SDK.Frontend.csproj:
//
// <Project Sdk="Microsoft.NET.Sdk.Web">
//  <PropertyGroup>
//    <TargetFramework>net5.0</TargetFramework>
//    <DxExtendStartupHost>False</DxExtendStartupHost>
//  </PropertyGroup>
//</auto-generated>
//------------------------------------------------------------------------------
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.JSInterop;
using Siesa.SDK.Frontend.Components.FormManager.Model;
using Siesa.SDK.Frontend.Components.Layout;

[assembly: HostingStartup(typeof(Siesa.SDK.Frontend.DevExpressHostingStartup))]

namespace Siesa.SDK.Frontend {
    public partial class DevExpressHostingStartup : IHostingStartup {
        void IHostingStartup.Configure(IWebHostBuilder builder) {
            builder.ConfigureServices((serviceCollection) => {
                serviceCollection.AddDevExpressBlazor();
            });
        }
    }

    public static class SiesaSecurityExtensions
    {
        public static void AddSiesaSDK(this IServiceCollection services)
        {
            services.AddDevExpressBlazor();
            services.AddScoped<StateContainer>();
            services.AddScoped<ILayoutService, LayoutService>();
        }

    }
}
