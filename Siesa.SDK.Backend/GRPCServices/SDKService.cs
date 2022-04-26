using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siesa.SDK.Business;
using Siesa.SDK.Protos;
using System.Reflection;
using Siesa.SDK.Shared.Business;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Backend.Access;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.Json;
using Google.Protobuf;
using Siesa.SDK.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Siesa.SDK.GRPCServices
{
    public class SDKService : Siesa.SDK.Protos.SDK.SDKBase
    {
        private readonly ILogger<SDKService> _logger;
        private readonly IServiceProvider _provider;

        private IAuthenticationService _authenticationService;
        
        public SDKService(ILogger<SDKService> logger, IServiceProvider provider, IAuthenticationService AuthenticationService)
        {
            _logger = logger;
            _provider = provider;
            _authenticationService = AuthenticationService;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new SDKContractResolver()
            };
        }

        private void SetCurrentUser(string token)
        {
            _authenticationService.SetToken(token);
        }

        public override Task<Protos.BusinessesResponse> GetBusinesses(Protos.GetBusinessesRequest request, ServerCallContext context)
        {
            var response = new Protos.BusinessesResponse();
            SetCurrentUser(request.CurrentUserToken);
            response.Businesses.AddRange(BusinessManager.Instance.Businesses.Values);
            return Task.FromResult(response);
        }

        public override Task<Protos.BusinessObjResponse> GetBusinessObj(Protos.GetBusinessObjRequest request, ServerCallContext context)
        {
            SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = ActivatorUtilities.CreateInstance(_provider,businessType);
            businessObj.SetProvider(_provider);

            var response = new Protos.BusinessObjResponse();
            var result = businessObj.Get(request.Id);

            businessObj.DetachedBaseObj();


            response.Response = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Task.FromResult(response);
        }

        private Type FindType(string name, bool includeSystemLibs = false)
        {
            Type businessType = null;
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!includeSystemLibs && (ass.FullName.StartsWith("System.") || ass.FullName.StartsWith("Microsoft.") || ass.FullName.StartsWith("mscorlib") || ass.FullName.StartsWith("netstandard") || ass.FullName.StartsWith("GRPC.")))
                    continue;
                businessType = ass.GetType(name);
                if (businessType != null)
                    break;
            }
            return businessType;
        }

        public override Task<Protos.SaveBusinessObjResponse> SaveBusinessObj(Protos.SaveBusinessObjRequest request, ServerCallContext context)
        {
            SetCurrentUser(request.CurrentUserToken);
            var response = new Protos.SaveBusinessObjResponse();
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            //dynamic x = ActivatorUtilities.CreateInstance(_provider,businessType);
            //json deserialize using Newtonsoft.Json
            dynamic businessObj = Newtonsoft.Json.JsonConvert.DeserializeObject(request.Business, businessType);
            businessObj.SetProvider(_provider);
            response.Id = businessObj.Save();
            return Task.FromResult(response);
        }

        public override Task<Protos.DeleteBusinessObjResponse> DeleteBusinessObj(Protos.DeleteBusinessObjRequest request, ServerCallContext context)
        {
            SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = ActivatorUtilities.CreateInstance(_provider,businessType);
            businessObj.SetProvider(_provider);

            var response = new Protos.DeleteBusinessObjResponse();
            var result = businessObj.Get(request.Id);
            businessObj.BaseObj = result;
            response.Id = businessObj.Delete();
            return Task.FromResult(response);
        }

        public override Task<Protos.LoadResult> GetDataBusinessObj(Protos.GetDataBusinessObjRequest request, ServerCallContext context)
        {
            SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = ActivatorUtilities.CreateInstance(_provider,businessType);
            businessObj.SetProvider(_provider);

            var result = businessObj.GetData(request.Skip, request.Take, request.Filter, request.OrderBy);
            var response = new Protos.LoadResult();
            response.TotalCount = result.TotalCount;
            response.GroupCount = result.GroupCount;
            response.Data.AddRange(((IEnumerable<object>)result.Data).Select(x => Newtonsoft.Json.JsonConvert.SerializeObject(x, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })));
            return Task.FromResult(response);
        }

        public override Task<Protos.LoadResult> EntityFieldSearch(Protos.EntityFieldSearchRequest request, ServerCallContext context)
        {
            SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = ActivatorUtilities.CreateInstance(_provider,businessType);
            businessObj.SetProvider(_provider);

            var result = businessObj.EntityFieldSearch(request.SearchText, request.Filters);
            var response = new Protos.LoadResult();
            response.TotalCount = result.TotalCount;
            response.GroupCount = result.GroupCount;
            response.Data.AddRange(((IEnumerable<object>)result.Data).Select(x => Newtonsoft.Json.JsonConvert.SerializeObject(x, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })));
            return Task.FromResult(response);
        }

        public override Task<ValidateAndSaveBusinessObjResponse> ValidateAndSaveBusinessObj(ValidateAndSaveBusinessObjRequest request, ServerCallContext context)
        {
            SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            //dynamic x = ActivatorUtilities.CreateInstance(_provider,businessType);
            //json deserialize using Newtonsoft.Json
            dynamic businessObj = Newtonsoft.Json.JsonConvert.DeserializeObject(request.Business, businessType);
            businessObj.SetProvider(_provider);

            var response = businessObj.ValidateAndSave();
            return Task.FromResult(response);

        }

        public override Task<Protos.MenuGroupsResponse> GetMenuGroups(Protos.GetMenuGroupsRequest request, ServerCallContext context)
        {
            SetCurrentUser(request.CurrentUserToken);
            dynamic dbFactory = _provider.GetService(typeof(IDbContextFactory<SDKContext>));
            List<E00130_MenuGroup> menuGroups = new();
            using (SDKContext dbContext = dbFactory.CreateDbContext())
            {
                menuGroups = dbContext.Set<E00130_MenuGroup>().AsQueryable().ToList();
            }

            var response = new Protos.MenuGroupsResponse
            {
                Response = JsonConvert.SerializeObject(menuGroups, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
            };
            return Task.FromResult(response);
        }

        public override Task<Protos.MenuItemsResponse> GetMenuItems(Protos.GetMenuItemsRequest request, ServerCallContext context)
        {
            SetCurrentUser(request.CurrentUserToken);
            dynamic dbFactory = _provider.GetService(typeof(IDbContextFactory<SDKContext>));
            List<E00131_Menu> menuItems = new();
            using (SDKContext dbContext = dbFactory.CreateDbContext())
            {
                menuItems = dbContext.Set<E00132_MenuGroupDetail>().AsQueryable().Where(x => x.MenuGroup.Rowid == request.GroupId).Include(x => x.Menu.SubMenus).ThenInclude(x=>x.SubMenus).ThenInclude(x => x.Feature).Select(x => x.Menu).ToList();
            }

            var response = new Protos.MenuItemsResponse
            {
                Response = JsonConvert.SerializeObject(menuItems, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
            };
            return Task.FromResult(response);
        }

        public override Task<Protos.ExposedMethodResponse> ExecuteExposedMethod(Protos.ExposedMethodRequest request, ServerCallContext context)
        {
            SetCurrentUser(request.CurrentUserToken);
            var response = new Protos.ExposedMethodResponse();
            try
            {
                BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
                var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
                dynamic businessObj = ActivatorUtilities.CreateInstance(_provider,businessType);
                businessObj.SetProvider(_provider);


                var methodName = request.MethodName;
                var method = businessType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                if(method == null)
                {
                    response.Success = false;
                    response.Errors.Add($"Method {methodName} not found in business {businessRegistry.Name}");
                    return Task.FromResult(response);
                }
                //check if method has custom attribute (SDKExposedMethod)
                var customAttributes = method.GetCustomAttributes(typeof(SDKExposedMethod), false);
                if (customAttributes.Length == 0)
                {
                    response.Success = false;
                    response.Errors.Add($"Method {methodName} not exposed in business {businessRegistry.Name}");
                    return Task.FromResult(response);
                }

                ICollection<ExposedMethodParam> methodParameters = request.Parameters;
                object[] parameters = new object[]{};
                bool useNamedParams = !(request.Parameters.Any(x => x.Name == null || x.Name == ""));
                if (useNamedParams)
                {
                    string[] paramNames = method.GetParameters().Select(x => x.Name).ToArray();
                    parameters = new object[paramNames.Length];
                    for (int i = 0; i < parameters.Length; ++i) 
                    {
                        parameters[i] = Type.Missing;
                    }
                    foreach (var methodParameter in methodParameters)
                    {
                        var paramName = methodParameter.Name;
                        var paramType = FindType(methodParameter.Type, true);
                        var paramValue = JsonConvert.DeserializeObject(methodParameter.Value, paramType);
                        var paramIndex = Array.IndexOf(paramNames, paramName);
                        if(paramIndex >= 0)
                        {
                            parameters[paramIndex] = paramValue;
                        }
                    }
                }
                else
                {
                    foreach (var methodParameter in methodParameters)
                    {
                        var paramName = methodParameter.Name;
                        var paramType = FindType(methodParameter.Type, true);
                        var paramValue = JsonConvert.DeserializeObject(methodParameter.Value, paramType);
                        parameters = parameters.Append(paramValue).ToArray();
                    }
                }
                var resultMethod = method.Invoke(businessObj, parameters);

                response.Success = resultMethod.GetType().GetProperty("Success").GetValue(resultMethod);
                if(response.Success){
                    var resultValue = resultMethod.GetType().GetProperty("Data").GetValue(resultMethod);
                    //response.Data = JsonConvert.SerializeObject(resultValue);
                    var compressor = typeof(StreamUtilities).GetMethod("Compress", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(resultValue.GetType());
                    response.Data = UnsafeByteOperations.UnsafeWrap(compressor.Invoke(null, new object[] { resultValue }));
                    //var bodyString = JsonConvert.SerializeObject(resultValue);
                    //response.Data = UnsafeByteOperations.UnsafeWrap(Encoding.UTF8.GetBytes(bodyString));
                    if(resultValue != null){
                        response.DataType = resultValue.GetType().FullName;
                    }                    
                }else{
                    response.Errors.AddRange(((IEnumerable<string>)resultMethod.GetType().GetProperty("Errors").GetValue(resultMethod)).Select(x => x));
                }
                
                return Task.FromResult(response);
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"Error executing exposed method {ex.Message}");
                return Task.FromResult(response);
            }
            
        } 

    }
}
