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
using System.Runtime.CompilerServices;
namespace Siesa.SDK.GRPCServices
{
    public class SDKService : Siesa.SDK.Protos.SDK.SDKBase
    {
        private readonly ILogger<SDKService> _logger;
        private readonly IServiceProvider _provider;

        private IAuthenticationService _authenticationService;
        private IBackendRouterService _backendRouterService;
        private ITenantProvider _tenantProvider;
        private IFeaturePermissionService _featurePermissionService;

        public SDKService(
            ILogger<SDKService> logger,
            IServiceProvider provider,
            IAuthenticationService AuthenticationService, IBackendRouterService backendRouterService,
            ITenantProvider tenantProvider,
            IFeaturePermissionService featurePermissionService
        )
        {
            _logger = logger;
            _provider = provider;
            _authenticationService = AuthenticationService;
            _tenantProvider = tenantProvider;
            _backendRouterService = backendRouterService;
            _featurePermissionService = featurePermissionService;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new SDKContractResolver()
            };
        }

        private async Task SetCurrentUser(string token)
        {
            await _authenticationService.SetToken(token);
            if(_tenantProvider != null && _authenticationService?.User != null)
            {
                await _tenantProvider.SetTenantByRowId(_authenticationService.User.RowIdDBConnection, _authenticationService.User.HostName);
            }
        }

        public override async Task<Protos.BusinessObjResponse> GetBusinessObj(Protos.GetBusinessObjRequest request, ServerCallContext context)
        {
            await SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = _backendRouterService.GetBackend(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = ActivatorUtilities.CreateInstance(_provider,businessType);
            businessObj.SetProvider(_provider);

            var response = new Protos.BusinessObjResponse();
            var result = businessObj.Get(request.Id, request.ExtraFields.ToList()); //request.Extrafields

            businessObj.DetachedBaseObj();


            response.Response = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return response;
        }

        private Type FindType(string name, bool includeSystemLibs = false)
        {
            if(string.IsNullOrEmpty(name))
            {
                return null;
            }
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

        public override async Task<Protos.SaveBusinessObjResponse> SaveBusinessObj(Protos.SaveBusinessObjRequest request, ServerCallContext context)
        {
            await SetCurrentUser(request.CurrentUserToken);
            var response = new Protos.SaveBusinessObjResponse();
            BusinessModel businessRegistry = _backendRouterService.GetBackend(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            //dynamic x = ActivatorUtilities.CreateInstance(_provider,businessType);
            //json deserialize using Newtonsoft.Json
            dynamic businessObj = Newtonsoft.Json.JsonConvert.DeserializeObject(request.Business, businessType);
            businessObj.SetProvider(_provider);
            response.Id = businessObj.Save();
            return response;
        }

        public override async Task<Protos.DeleteBusinessObjResponse> DeleteBusinessObj(Protos.DeleteBusinessObjRequest request, ServerCallContext context)
        {
            await SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = _backendRouterService.GetBackend(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = ActivatorUtilities.CreateInstance(_provider,businessType);
            businessObj.SetProvider(_provider);

            var response = new Protos.DeleteBusinessObjResponse();
            var result = businessObj.Get(request.Id);
            businessObj.BaseObj = result;
            response = businessObj.Delete();
            return response;
        }

        public override async Task<Protos.LoadResult> GetDataBusinessObj(Protos.GetDataBusinessObjRequest request, ServerCallContext context)
        {
            await SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = _backendRouterService.GetBackend(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = ActivatorUtilities.CreateInstance(_provider,businessType);
            businessObj.SetProvider(_provider);

            var result = businessObj.GetData(request.Skip, request.Take, request.Filter, request.OrderBy, null, 
                                            request.IncludeCount, extraFields: request.ExtraFields.ToList());
            var response = new Protos.LoadResult();
            response.TotalCount = result.TotalCount;
            response.GroupCount = result.GroupCount;
            response.Data.AddRange(((IEnumerable<object>)result.Data).Select(x => Newtonsoft.Json.JsonConvert.SerializeObject(x, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })));
            return response;
        }

        public override async Task<Protos.LoadResult> GetUDataBusinessObj(Protos.GetUDataRequest request, ServerCallContext context)
        {
            await SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = _backendRouterService.GetBackend(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = ActivatorUtilities.CreateInstance(_provider,businessType);
            businessObj.SetProvider(_provider);

            var result = businessObj.GetUData(request.Skip, request.Take, request.Filter, request.UFilter, request.OrderBy, null, request.IncludeCount, selectFields: request.SelectFields.ToList());

            var response = new Protos.LoadResult();
            response.TotalCount = result.TotalCount;
            response.GroupCount = result.GroupCount;
            response.Data.AddRange(((IEnumerable<object>)result.Data).Select(x => Newtonsoft.Json.JsonConvert.SerializeObject(x, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })));
            return response;
        }

        public override async Task<Protos.LoadResult> EntityFieldSearch(Protos.EntityFieldSearchRequest request, ServerCallContext context)
        {
            await SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = _backendRouterService.GetBackend(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = ActivatorUtilities.CreateInstance(_provider,businessType);
            businessObj.SetProvider(_provider);

            var result = businessObj.EntityFieldSearch(request.SearchText, request.Filters, request.Top, request.OrderBy, 
            request.ExtraFields.ToList());
            var response = new Protos.LoadResult();
            response.TotalCount = result.TotalCount;
            response.GroupCount = result.GroupCount;
            response.Data.AddRange(((IEnumerable<object>)result.Data).Select(x => Newtonsoft.Json.JsonConvert.SerializeObject(x, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })));
            return response;
        }

        public override async Task<ValidateAndSaveBusinessObjResponse> ValidateAndSaveBusinessObj(ValidateAndSaveBusinessObjRequest request, ServerCallContext context)
        {
            await SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = _backendRouterService.GetBackend(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            //dynamic x = ActivatorUtilities.CreateInstance(_provider,businessType);
            //json deserialize using Newtonsoft.Json
            dynamic businessObj = Newtonsoft.Json.JsonConvert.DeserializeObject(request.Business, businessType);
            businessObj.SetProvider(_provider);

            var response = businessObj.ValidateAndSave();
            return response;

        }

        public override Task<ValidateAndSaveBusinessObjResponse> ValidateAndSaveBusinessMultiObj(ValidateAndSaveBusinessMultiObjRequest request, ServerCallContext context)
        {
            SetCurrentUser(request.CurrentUserToken);
            BusinessModel businessRegistry = _backendRouterService.GetBackend(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            //dynamic x = ActivatorUtilities.CreateInstance(_provider,businessType);
            //json deserialize using Newtonsoft.Json
            dynamic businessObj = Newtonsoft.Json.JsonConvert.DeserializeObject(request.Business, businessType);
            businessObj.SetProvider(_provider);

            List<dynamic> listBaseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(request.ListBaseObj);

            var response = businessObj.ValidateAndSave(listBaseObj);
            return Task.FromResult(response);

        }

        public override async Task<Protos.MenuGroupsResponse> GetMenuGroups(Protos.GetMenuGroupsRequest request, ServerCallContext context)
        {
            await SetCurrentUser(request.CurrentUserToken);
            dynamic dbFactory = _provider.GetService(typeof(IDbContextFactory<SDKContext>));
            List<E00060_Suite> menuGroups = new();
            using (SDKContext dbContext = dbFactory.CreateDbContext())
            {
                menuGroups = dbContext.Set<E00060_Suite>().AsQueryable().ToList();
            }

            var response = new Protos.MenuGroupsResponse
            {
                Response = JsonConvert.SerializeObject(menuGroups, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
            };
            return response;
        }

        public override async Task<Protos.MenuItemsResponse> GetMenuItems(Protos.GetMenuItemsRequest request, ServerCallContext context)
        {
            await SetCurrentUser(request.CurrentUserToken);
            dynamic dbFactory = _provider.GetService(typeof(IDbContextFactory<SDKContext>));
            List<E00061_Menu> menuItems = new();
            using (SDKContext dbContext = dbFactory.CreateDbContext())
            {
                menuItems = dbContext.Set<E00062_SuiteMenu>().AsQueryable()
                    .Where(x => x.Suite.Rowid == request.GroupId)
                    .Include(x => x.Menu.SubMenus)
                    .ThenInclude(x=>x.SubMenus)
                    .ThenInclude(x => x.Feature)
                    .Select(x => x.Menu)
                    .ToList();
            }

            var response = new Protos.MenuItemsResponse
            {
                Response = JsonConvert.SerializeObject(menuItems, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })
            };
            return response;
        }

        public override async Task<Protos.ExposedMethodResponse> ExecuteExposedMethod(Protos.ExposedMethodRequest request, ServerCallContext context)
        {
            await SetCurrentUser(request.CurrentUserToken);
            var response = new Protos.ExposedMethodResponse();
            try
            {
                BusinessModel businessRegistry = _backendRouterService.GetBackend(request.BusinessName);
                var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
                dynamic businessObj = ActivatorUtilities.CreateInstance(_provider,businessType);
                businessObj.SetProvider(_provider);


                var methodName = request.MethodName;
                var method = businessType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                if(method == null)
                {
                    response.Success = false;
                    response.Errors.Add($"Method {methodName} not found in business {businessRegistry.Name}");
                    return response;
                }
                //check if method has custom attribute (SDKExposedMethod)
                var customAttributes = method.GetCustomAttributes(typeof(SDKExposedMethod), false);
                if (customAttributes.Length == 0)
                {
                    response.Success = false;
                    response.Errors.Add($"Method {methodName} not exposed in business {businessRegistry.Name}");
                    return response;
                }
                var exposedMethod = customAttributes.FirstOrDefault() as SDKExposedMethod;
                if(exposedMethod.Permissions.Length > 0){
                    var hasPermission = true;
                    if(_featurePermissionService != null){
                        try{
                            List<string> permissions = exposedMethod.Permissions.ToList();
                            hasPermission =  _featurePermissionService.CheckUserActionPermissions(request.BusinessName,
                                permissions, _authenticationService).GetAwaiter().GetResult();
                            if(!hasPermission){
                                response.Success = false;
                                response.Errors.Add("Custom.Generic.Unauthorized");
                                return response;
                            }
                        }catch(Exception e){
                        }
                    }
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
                        object? paramValue = null;
                        if(paramType != null)
                        {
                            paramValue = JsonConvert.DeserializeObject(methodParameter.Value, paramType);
                        }
                        parameters = parameters.Append(paramValue).ToArray();
                    }

                    if (parameters.Length != method.GetParameters().Length)
                    {
                        //add missing parameters
                        var missingParams = method.GetParameters().Length - parameters.Length;
                        for (int i = 0; i < missingParams; i++)
                        {
                            parameters = parameters.Append(Type.Missing).ToArray();
                        }
                    }
                }
                var resultMethod = method.Invoke(businessObj, parameters);

                //check if methods returns a async task
                if (method.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false).Length > 0)
                {
                    //wait for task to complete
                    var task = (Task)resultMethod;
                    task.Wait();
                    resultMethod = task.GetType().GetProperty("Result").GetValue(task);
                }

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
                
                return response;
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Errors.Add($"Error executing exposed method {ex.Message}");
                return response;
            }
            
        }
    }
}
