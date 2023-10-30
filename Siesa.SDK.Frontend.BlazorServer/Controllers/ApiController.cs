using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.Json;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Siesa.SDK.Frontend.Controllers
{
    public class ApiController : Controller
    {
        private IServiceProvider ServiceProvider { get; set; }
        private IAuthenticationService AuthenticationService { get; set; }
        private IBackendRouterService BackendRouterService { get; set; }

        public ApiController(IServiceProvider ServiceProvider, IAuthenticationService AuthenticationService, IBackendRouterService backendRouterService)
        {
            this.ServiceProvider = ServiceProvider;
            try{
                //replace NavigationManager service in ServiceProvider with the one from the controller
                var navigationManager = ServiceProvider.GetRequiredService<NavigationManager>();
                navigationManager.GetType().GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(navigationManager, new object[] { "https://localhost:5002/", "https://localhost:5002/" });
            }catch(Exception e){
            }
            this.AuthenticationService = AuthenticationService;
            this.BackendRouterService = backendRouterService;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                ContractResolver = new SDKContractResolver()
            };
        }

        //RedirectToDocs
        public ActionResult RedirectToDocs(string url)
        {
            return Redirect("https://sdk-docs.siesadev.com/");
        }

        private object[] GetArgs(ParameterInfo[] parameters, IEnumerable<KeyValuePair<string, StringValues>> collection = null)
        {
            string[] paramNames = parameters.Select(x => x.Name).ToArray();
            object[] args = new object[paramNames.Length];


            if (parameters.Length == 0)
            {
                return args;
            }
            collection.Where(
                x => paramNames.Contains(x.Key)
            ).ToList().ForEach(
                x =>
                {
                    int index = Array.IndexOf(paramNames, x.Key);
                    var paramType = parameters[index].ParameterType;
                    if(!StringValues.IsNullOrEmpty(x.Value) || paramType == typeof(string)){
                        args[index] = Convert.ChangeType(x.Value.ToString(), paramType);
                    }
                });
            
            if(collection.GetType() == typeof(FormCollection)){
                parameters.Where(x => x.ParameterType == typeof(IFormFile)).ToList().ForEach(x =>
                {
                    int index = Array.IndexOf(paramNames, x.Name);
                    args[index] = ((FormCollection)collection).Files.Where(y => y.Name == x.Name).FirstOrDefault();
                    
                });
            }

            return args;
        }

        private ContentResult ReturnError(HttpResponse response, string message, int statusCode = 400)
        {
            var jsonResponse = new Dictionary<string, object>();
            jsonResponse.Add("status", false);
            jsonResponse.Add("message", message);
            string json;
            try
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResponse, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            catch (System.Exception e)
            {
                var errorResponse = new Dictionary<string, object>();
                errorResponse.Add("status", false);
                errorResponse.Add("message", e.ToString());
                json = Newtonsoft.Json.JsonConvert.SerializeObject(errorResponse);
            }
            Response.StatusCode = statusCode;
            return Content(json, "application/json");
        }

        public async Task<ActionResult> Index(string blname, string blaction)
        {
            //get auth token from headers
            string authToken = "";
            var sessionId = "";
            short rowidDbConnection = 1;
            Request.Cookies.TryGetValue("sdksession", out sessionId);
            if(!Request.Cookies.TryGetValue("selectedConnection", out string rowidDbConnectionStr) && Request.Headers.TryGetValue("x-sdk-selected-connection", out StringValues rowidDbConnectionHeader)){
                rowidDbConnectionStr = rowidDbConnectionHeader.ToString();
            }

            if (!string.IsNullOrEmpty(rowidDbConnectionStr)){
                short.TryParse(rowidDbConnectionStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out rowidDbConnection);
            }
            if (!string.IsNullOrEmpty(sessionId)){
                var blSession = BackendRouterService.GetSDKBusinessModel("BLSession", AuthenticationService);
                var response = await blSession.Call("GetSession", sessionId, rowidDbConnection, Request.Host.Host).ConfigureAwait(true);
                if(response.Success){
                    authToken = response.Data;
                }
            }
            
            if (string.IsNullOrEmpty(authToken))
            {
                return ReturnError(Response, "BLFlex.Error.NoTokenProvided", 401);
            }

            AuthenticationService.SetToken(authToken);
            if (AuthenticationService.User == null)
            {
                return ReturnError(Response, "BLFlex.Error.NoTokenValid", 401);
            }

            var jsonResponse = new Dictionary<string, object>();
            jsonResponse.Add("status", true);
            int HTTPCodeResponse = 200;

            SDKBusinessModel businessModel;
            Type businessType;
            dynamic BusinessObj;

            businessModel = BackendRouterService.GetSDKBusinessModel(blname, AuthenticationService);
            if (businessModel != null)
            {
                try
                {
                    businessType = Utilities.SearchType(businessModel.Namespace + "." + businessModel.Name);
                    BusinessObj = ActivatorUtilities.CreateInstance(ServiceProvider, businessType);
                    BusinessObj.BusinessName = blname;

                    MethodInfo method = BusinessObj.GetType().GetMethod(blaction);
                    if (method == null)
                    {
                        return ReturnError(Response, "Method not found", 404);
                    }

                    var customAttributes = method.GetCustomAttributes(typeof(SDKApiMethod), false);
                    if (customAttributes.Length == 0)
                    {
                        return ReturnError(Response, "Method not found", 404);
                    }
                    else
                    {
                        var customAttribute = customAttributes[0] as SDKApiMethod;

                        //get parameters
                        ParameterInfo[] parameters = method.GetParameters();
                        object[] args = new object[parameters.Length];
                        var requestMethod = Request.Method;

                        if (requestMethod != customAttribute.HTTPMethod)
                        {
                            jsonResponse["status"] = false;
                            jsonResponse["message"] = "Method not found";
                            HTTPCodeResponse = 404;
                        }
                        else
                        {
                            if (requestMethod == "GET")
                            {
                                args = GetArgs(parameters, Request.Query);
                            }
                            else if (requestMethod == "POST")
                            {
                                if (Request.HasFormContentType)
                                {
                                    args = GetArgs(parameters, Request.Form);
                                }
                                else
                                {
                                    if (Request.Body.CanRead)
                                    {
                                        try
                                        {
                                            var jsonBody = await new StreamReader(Request.Body).ReadToEndAsync();
                                            var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonBody);
                                            IEnumerable<KeyValuePair<string, StringValues>> form = jsonObj.Select(x => new KeyValuePair<string, StringValues>(x.Key, x.Value==null?null:x.Value.ToString()));
                                            args = GetArgs(parameters, form);
                                        }
                                        catch (System.Exception)
                                        {
                                                
                                        }
                                    }
                                }
                            }

                            object response;
                            try
                            {
                                response = method.Invoke(BusinessObj, args);
                                if (method.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false).Length > 0)
                                {
                                    //wait for task to complete
                                    var task = (Task)response;
                                    task.Wait();
                                    response = task.GetType().GetProperty("Result").GetValue(task);
                                }
                                jsonResponse.Add("data", response);
                            }
                            catch (System.ArgumentException)
                            {
                                jsonResponse["status"] = false;
                                jsonResponse.Add("message", "Invalid parameters");
                                HTTPCodeResponse = 400;
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    jsonResponse["status"] = false;
                    jsonResponse.Add("message", e.Message);
                    HTTPCodeResponse = 500;
                }
            }
            else
            {
                jsonResponse["status"] = false;
                jsonResponse.Add("message", "404 Not Found.");
                HTTPCodeResponse = 404;
            }
            string json;
            try
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResponse, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            catch (System.Exception e)
            {
                HTTPCodeResponse = 500;
                var errorResponse = new Dictionary<string, object>();
                errorResponse.Add("status", false);
                errorResponse.Add("message", e.ToString());
                json = Newtonsoft.Json.JsonConvert.SerializeObject(errorResponse);
            }
            Response.StatusCode = HTTPCodeResponse;
            return Content(json, "application/json");
        }

        [HttpPost]
        public async Task<ActionResult> GetSessionToken([FromForm] string accessToken)
        {
            var jsonResponse = new Dictionary<string, object>();
            string json;
            try
            {
                if ((Request.Headers.TryGetValue("x-sdk-selected-connection", out StringValues rowidConnectionStringValues)) && short.TryParse(rowidConnectionStringValues, NumberStyles.Integer, CultureInfo.InvariantCulture, out short rowidConnection)){
                    string sessionToken = await AuthenticationService.LoginSessionByToken(accessToken, rowidConnection).ConfigureAwait(true);
                    jsonResponse.Add("status", true);
                    jsonResponse.Add("data", sessionToken);
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResponse, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                    return Content(json, "application/json");
                }else{
                    return ReturnError(Response, "Invalid value for x-sdk-selected-connection header.", 400);
                }
            }catch (Exception e){
                return ReturnError(Response, e.Message, 401);
            }
        }
    }
}