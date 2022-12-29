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
using System.Linq;
using Siesa.SDK.Shared.DataAnnotations;
using System.Reflection;

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
            string BlNameSpace = "";
            string MethodName = "";
            if (commandText.Split('-').Length > 1)
            {
                var commandTextSplit = commandText.Split('-');
                BlNameSpace = commandTextSplit[0];
                MethodName = commandTextSplit[1];
            }else
            {
                BlNameSpace = commandText;
            }
           
            Type BLType = Utilities.SearchType(BlNameSpace, true);

            dynamic response = null;

            if (BLType != null)
            {
                dynamic BLInstance =  ActivatorUtilities.CreateInstance(_serviceProvider, BLType);

                if (!string.IsNullOrEmpty(MethodName))
                {
                    MethodInfo method = BLInstance.GetType().GetMethod(MethodName);
                    if (method != null)
                    {
                        response = method.Invoke(BLInstance, null);
                    }
                    return response;
                }else
                {
                    response = BLInstance.GetData(null,null);
                }

                return response.Data;
            }
            return new List<dynamic>();
        }

        internal Type GetBLType(string commandText)
        {
            string BlNameSpace = "";
            string MethodName = "";
            if (commandText.Split('-').Length > 1)
            {
                var commandTextSplit = commandText.Split('-');
                BlNameSpace = commandTextSplit[0];
                MethodName = commandTextSplit[1];
            }else{
                BlNameSpace = commandText;
            }

            Type BLType = Utilities.SearchType(BlNameSpace, true);

            if (BLType != null)
            {
                if (!string.IsNullOrEmpty(MethodName))
                {
                    MethodInfo method = BLType.GetMethod(MethodName);
                    if (method != null)
                    {
                        return method.ReturnType.GetGenericArguments()[0];
                    }
                }else
                {
                    return BLType.GetProperty("BaseObj").PropertyType;
                }
            }
            

            return null;
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