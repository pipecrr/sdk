using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Siesa.SDK.Shared.Json;
using Siesa.SDK.Shared.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Siesa.SDK.Frontend.Controllers
{
    public class ApiController : Controller
    {
        private IServiceProvider ServiceProvider { get; set; }
        private IAuthenticationService AuthenticationService { get; set; }

        public ApiController(IServiceProvider ServiceProvider, IAuthenticationService AuthenticationService)
        {
            this.ServiceProvider = ServiceProvider;
            this.AuthenticationService = AuthenticationService;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                ContractResolver = new SDKContractResolver()
            };
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
                    args[index] = Convert.ChangeType(x.Value.ToString(), paramType);
                });

            return args;
        }

        public async Task<ActionResult> Index(string blname, string blaction)
        {
            //TODO: AUTH
            // if(AuthenticationService?.User == null)
            // {
            //     Console.WriteLine("user not found");
            //     return null;
            // }
            var jsonResponse = new Dictionary<string, object>();
            jsonResponse.Add("status", true);
            int HTTPCodeResponse = 200;

            BusinessFrontendModel businessModel;
            Type businessType;
            dynamic BusinessObj;

            BusinessManagerFrontend.Instance.Businesses.TryGetValue(blname, out businessModel);
            if (businessModel != null)
            {
                try
                {
                    businessType = Utils.Utils.SearchType(businessModel.Namespace + "." + businessModel.Name);
                    BusinessObj = ActivatorUtilities.CreateInstance(ServiceProvider, businessType);
                    BusinessObj.BusinessName = blname;

                    MethodInfo method = BusinessObj.GetType().GetMethod(blaction);
                    if (method == null)
                    {
                        Console.WriteLine("method not found");
                        return null;
                    }

                    //get parameters
                    ParameterInfo[] parameters = method.GetParameters();
                    object[] args = new object[parameters.Length];
                    var requestMethod = Request.Method;
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
                            if(Request.ContentType == "application/json" && Request.Body.CanRead)
                            {
                                var jsonBody = await new StreamReader(Request.Body).ReadToEndAsync();
                                var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonBody);
                                IEnumerable<KeyValuePair<string, StringValues>> form = jsonObj.Select(x => new KeyValuePair<string, StringValues>(x.Key, x.Value.ToString()));
                                args = GetArgs(parameters, form);
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
                        jsonResponse["status"] = "error";
                        jsonResponse.Add("message", "Invalid parameters");
                        HTTPCodeResponse = 400;
                    }
                }
                catch (System.Exception e)
                {
                    jsonResponse["status"] = "error";
                    jsonResponse.Add("message", e.ToString());
                    HTTPCodeResponse = 500;
                }
            }
            else
            {
                jsonResponse["status"] = "error";
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
    }
}