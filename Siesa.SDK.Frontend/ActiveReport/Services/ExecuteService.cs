using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Shared.Application;
using Siesa.SDK.Shared.Utilities;

namespace Siesa.SDK.Frontend.Report.Services
{
    public static class ExecuteService
    {

        private static dynamic BusinessInstance(string business_name)
        {
            dynamic BusinessInstance = "";
            IServiceProvider ServiceProvider = SDKApp.GetServiceProvider();

            if (ServiceProvider != null)
            {
                var businessModel = BackendRouterService.Instance.GetSDKBusinessModel(business_name, null);
                if (businessModel != null)
                {
                    var BLInstance = ActivatorUtilities.CreateInstance(ServiceProvider,
                    Utilities.SearchType($"{businessModel.Namespace}.{businessModel.Name}", true));
                    BusinessInstance = BLInstance != null ? BLInstance : null;
                }
            }
            return BusinessInstance;
        }
        private static object _CallMethod(string business_name, string function_name, object[] parameters = null)
        {
            object Response = "";

            object BLInstance = BusinessInstance(business_name);
            if (BLInstance != null)
            {

                MethodInfo method = BLInstance.GetType().GetMethod(function_name);

                if (method != null)
                {
                    Response = method.Invoke(BLInstance, parameters);

                    if (method.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false).Length > 0)
                    {
                        var task = (Task)Response;
                        task.Wait();
                        Response = task.GetType().GetProperty("Result").GetValue(task);
                    }
                }
            }
            return Response;
        }
        public static object CallMethod(string business_name, string function_name, params object[] parameters)
        {
            return _CallMethod(business_name, function_name, parameters);
        }

        public static object CallMethod(string business_name, string function_name)
        {
            return _CallMethod(business_name, function_name, null);
        }
    }
}