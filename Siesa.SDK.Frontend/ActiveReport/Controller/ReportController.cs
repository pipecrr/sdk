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

        internal IEnumerable<object>  GetBLData(string commandText, string Filters="")
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
                        Response = method.Invoke(BLInstance, null);
                    }
                }else
                {
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

        internal string GetFilters(DbParameterCollection ParametersCollection, string commandText)
        {
            string Filters = string.Empty;

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

        /*internal List<string> GetComandText(string _comandText)
        {
            List<string> Namespaces = new List<string>();

            if (_commandText.Split('-').Length > 1)
            {
                var commandTextSplit = _commandText.Split('-');
                Namespaces.Add(commandTextSplit[0]);
                Namespaces.Add(commandTextSplit[1]);
            }else{
                Namespaces.Add(_commandText);
            }

            return Namespaces;
        }*/
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