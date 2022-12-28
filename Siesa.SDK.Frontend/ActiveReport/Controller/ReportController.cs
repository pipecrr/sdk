using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Entities;
using Siesa.SDK.Frontend.Application;
using Siesa.SDK.Shared.Application;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
using Siesa.SDK.Business;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Report.Controllers
{
    public class InternalSDKReportProvider {
        
        private IBackendRouterService _backendRouterService;
        private IResourceManager _resourceManager;

        private IServiceProvider _serviceProvider;
        public InternalSDKReportProvider(IBackendRouterService backendRouterService, IResourceManager resourceManager, IServiceProvider serviceProvider)
        {
            _backendRouterService = backendRouterService;
            _resourceManager = resourceManager;
            _serviceProvider = serviceProvider;
        }

        internal IEnumerable<object>  GetBLData(string commandText)
        {
            Type BLType = Utilities.SearchType(commandText, true);

            if (BLType != null)
            {
                dynamic BLInstance =  ActivatorUtilities.CreateInstance(_serviceProvider, BLType);

               // 

                var response = BLInstance.GetData(null,null);

                return response.Data;
            }

            return new List<dynamic>();
        }

        internal Type GetBLType(string commandText)
        {
            Type BLType = Utilities.SearchType(commandText, true);

            return BLType.GetProperty("BaseObj").PropertyType;
        }
    }

    public class SDKReportProvider: DbProviderFactory {
        

        //public static SDKReportProvider Instance{get;} = new SDKReportProvider();
        private InternalSDKReportProvider _internalSDKReportProvider;

        public SDKReportProvider()
        {
            _internalSDKReportProvider = ActivatorUtilities.CreateInstance(SDKApp.GetServiceProvider(), typeof(InternalSDKReportProvider)) as InternalSDKReportProvider;
        }

        public override DbConnection CreateConnection()
        {
            return new SDKReportConnection(_internalSDKReportProvider);
        }

    }
}