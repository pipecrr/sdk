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
using Siesa.SDK.Frontend.Components.FormManager.Fields;
using Siesa.SDK.Protos;
using System.Runtime.CompilerServices;

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

        internal IEnumerable<object>  GetBLData(string commandText, DbParameterCollection ParametersCollection)
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

           
            dynamic Request = new List<dynamic>();
            dynamic Response = new List<dynamic>();
            Type BLType = Utilities.SearchType(BlNameSpace, true);

            if (BLType != null)
            {
                dynamic BLInstance =  ActivatorUtilities.CreateInstance(_serviceProvider, BLType);

                if (!string.IsNullOrEmpty(MethodName))
                {
                    MethodInfo method = BLInstance.GetType().GetMethod(MethodName);
                    if (method != null)
                    {
                        object[] parameters = GetParameters(BlNameSpace, MethodName, ParametersCollection);
  
                            Response = method.Invoke(BLInstance, parameters);
                            if (method.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false).Length > 0)
                            {
                                //wait for task to complete
                                var task = (Task)Response;
                                task.Wait();
                                Response = task.GetType().GetProperty("Result").GetValue(task);
                            }
                    }
                }else
                {
                    string Filters = GetFilters(BlNameSpace,ParametersCollection);
                    if (!string.IsNullOrEmpty(Filters))
                    {
                        Request = BLInstance.GetData(null,null,Filters);
                    }else
                    {
                        Request = BLInstance.GetData(null,null);
                    }

                    Response = Request.Data;
                }
            }
            return Response;
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

            Type response = null;
            Type BLType = Utilities.SearchType(BlNameSpace, true);

            if (BLType != null)
            {
                if (!string.IsNullOrEmpty(MethodName))
                {
                    MethodInfo method = BLType.GetMethod(MethodName);
                    if (method != null)
                    {
                        response = method.ReturnType.GetGenericArguments()[0];
                    }
                }else
                {
                    response = BLType.GetProperty("BaseObj").PropertyType;
                }
            }
            return response;
        }

        internal object[] GetParameters(string BlNameSpace, string MethodName,DbParameterCollection ParametersCollection)
        {

            Type BLType = Utilities.SearchType(BlNameSpace, true);

            object[] parameters = new object[]{};
            if (BLType != null){
                dynamic BLInstance =  ActivatorUtilities.CreateInstance(_serviceProvider, BLType);
                
                MethodInfo method = BLInstance.GetType().GetMethod(MethodName);
                
                var Parameters = ParametersCollection.Cast<SDKReportParameter>();
                
                ICollection<ExposedMethodParam> methodParameters = method.GetParameters().Select(x => new ExposedMethodParam
                {
                    Name = x.Name,
                    Type = x.ParameterType.ToString(),
                    Value = x.DefaultValue.ToString()
                }).ToList();

                if (Parameters != null && Parameters.Any())
                {    
                    foreach (var item in Parameters)
                    {
                        var param = methodParameters.Where(x=> x.Name == item.ParameterName).FirstOrDefault();

                        if (param != null)
                        {
                            var paramType = Utilities.SearchType(param.Type, true);
                            if (paramType != null)
                            {
                                var paramValue = Convert.ChangeType(item.Value, paramType);
                                if (paramValue != null)
                                {
                                    parameters = parameters.Append(paramValue).ToArray();
                                }
                            }
                        }
                    }
                }   
            }
            return parameters;
        }

        internal string GetFilters(string BlNameSpace, DbParameterCollection ParametersCollection)
        {
            string Filters = string.Empty;

            Type BLType = Utilities.SearchType(BlNameSpace, true);

            if (BLType != null)
            {
                var Entity = BLType.GetProperty("BaseObj").PropertyType;
                var EntityProperties = Entity.GetProperties();

                var Parameters = ParametersCollection.Cast<SDKReportParameter>();

                if (Parameters != null  && Parameters.Count() > 0)
                {    
                    foreach (var item in Parameters)
                    {
                        var Property = EntityProperties.Where(x=> x.Name == item.ParameterName).FirstOrDefault();

                        if (Property != null)
                        {
                           Type propertyType =  Property.PropertyType;
                           bool IsNullable = false;
                           FieldTypes fieldType = FieldTypes.Unknown;

                            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                propertyType = propertyType.GetGenericArguments()[0];
                                IsNullable = true;
                            }
                            switch (propertyType.Name)
                            {
                                case "String":
                                    fieldType = FieldTypes.CharField;
                                    break;
                                case "Int64":
                                    fieldType = FieldTypes.BigIntegerField;
                                    break;
                                case "Int32":
                                    fieldType = FieldTypes.IntegerField;
                                    break;
                                case "Int16":
                                    fieldType = FieldTypes.SmallIntegerField;
                                    break;
                                case "Byte":
                                    fieldType = FieldTypes.ByteField;
                                    break;
                                case "Decimal":
                                    fieldType = FieldTypes.DecimalField;
                                    break;
                                case "DateTime":
                                    fieldType = FieldTypes.DateTimeField;
                                    break;
                                case "TimeOnly":
                                case "TimeSpan":
                                    fieldType = FieldTypes.TimeField;
                                    break;
                                case "DateOnly":
                                    fieldType = FieldTypes.DateField;
                                    break;
                                case "Boolean":
                                    fieldType = FieldTypes.BooleanField;
                                    break;
                                case "EntityTextField":
                                    fieldType = FieldTypes.TextField;
                                    break;
                                default:
                                    fieldType = FieldTypes.Unknown;
                                    break;
                            }
                           switch (fieldType)
                           {
                            case FieldTypes.CharField:
                            case FieldTypes.TextField:
                                Filters = $"({item.ParameterName} == null ? \"\" : {item.ParameterName}).ToLower().Contains(\"{item.Value}\".ToLower())";
                                break;

                            case FieldTypes.IntegerField:
                            case FieldTypes.DecimalField:
                            case FieldTypes.SmallIntegerField:
                            case FieldTypes.BigIntegerField:
                            case FieldTypes.ByteField:
                                if (!IsNullable)
                                {
                                    Filters = $"{item.ParameterName} == {item.Value}";
                                }
                                else
                                {
                                    Filters = $"({item.ParameterName} == null ? 0 : {item.ParameterName}) == {item.Value}";
                                }
                                break;
                
                            default:
                            break;
                           }
                        }
                    }
                }
            }

            return Filters;
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