using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Grpc.Core;
using Newtonsoft.Json;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Backend;
using Siesa.SDK.Shared.Business;
using Siesa.SDK.Shared.Json;

namespace Siesa.SDK.Shared.Services
{
    public class SDKBusinessModel
    {
        private IAuthenticationService AuthenticationService { get; set; }
        private IBackendRouterService BackendRouterService {get;set;}
        public string Name { get; set; }
        public string Namespace { get; set; }

        public SDKBusinessModel(string name, string namespaceName, IAuthenticationService authenticationService = null, IBackendRouterService backendRouterService=null)
        {
            Name = name;
            Namespace = namespaceName;
            AuthenticationService = authenticationService;
            BackendRouterService = backendRouterService;
        }

        public void SetAuthenticationService(IAuthenticationService authenticationService, IBackendRouterService backendRouterService)
        {
            //TODO: ver si se puede eliminar
            AuthenticationService = authenticationService;
            BackendRouterService = backendRouterService;
        }

        public BackendRegistry Backend {get { return BackendRouterService.GetBackendRegistry(Name, AuthenticationService); } }

        public dynamic Save(dynamic obj)
        {
            return Backend.SaveBusiness(Name, obj);
        }

        public dynamic ValidateAndSave(dynamic obj, IList<dynamic> listBaseObj = null)
        {
            if(listBaseObj != null && listBaseObj.Count > 0)
            {
                return Backend.ValidateAndSaveBusinessMulti(Name, obj, listBaseObj);
            }
            return Backend.ValidateAndSaveBusiness(Name, obj);
        }

        public dynamic Delete(Int64 id)
        {
            return Backend.DeleteBusinessObj(Name, id);
        }

        public async Task<dynamic> DeleteAsync(Int64 id)
        {
            return await Backend.DeleteBusinessObj(Name, id);
        }

        public dynamic Get(Int64 id, List<string> extraFields = null)
        {
            return Backend.GetBusinessObj(Name, id, extraFields);
        }

        private async Task<ActionResult<dynamic>> transformCallResponse(ExposedMethodResponse grpcResult){
            if(grpcResult == null){
                return new ActionResult<dynamic>(){
                    Success = false,
                    Errors = new List<string>(){
                        "Custom.Backend.NoResponse"
                    }
                };
            }
            var response = new ActionResult<dynamic>() { Success = grpcResult.Success, Errors = grpcResult.Errors };
            if(response.Success){
                Type t = null;
                if(!string.IsNullOrEmpty(grpcResult.DataType)){
                     t = Utilities.Utilities.SearchType(grpcResult.DataType, true);
                }
   
                var byteString = grpcResult.Data;
                ByteArrayContent content;
                if (MemoryMarshal.TryGetArray(byteString.Memory, out var segment))
                {
                    // Success. Use the ByteString's underlying array.
                    content = new ByteArrayContent(segment.Array, segment.Offset, segment.Count);
                }
                else
                {
                    // TryGetArray didn't succeed. Fall back to creating a copy of the data with ToByteArray.
                    content = new ByteArrayContent(byteString.ToByteArray());
                }
                var decompressor = typeof(StreamUtilities).GetMethod("Decompress", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(t);
                Stream stream = await content.ReadAsStreamAsync();
                response.Data = decompressor.Invoke(null, new object[] { stream });


            }
            return response;
        }

        public async Task<ActionResult<dynamic>> Call(string method, params dynamic[] args)
        {
            List<ExposedMethodParam> exposedMethodParams = new List<ExposedMethodParam>();
            foreach (var arg in args)
            {
                var value = JsonConvert.SerializeObject(arg, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                string typeFullName = null;
                if(arg != null)
                {
                    typeFullName = arg.GetType().FullName;
                }
                if(typeFullName == null)
                {
                    typeFullName = "";
                }
                exposedMethodParams.Add(new ExposedMethodParam() { Name = "", Value = value, Type = typeFullName });
            }
            var grpcResult = await Backend.CallBusinessMethod(Name, method, exposedMethodParams);
            return await transformCallResponse(grpcResult);
        }

        public async Task<ActionResult<dynamic>> Call(string method, Dictionary<string, dynamic> args)
        {
            List<ExposedMethodParam> exposedMethodParams = new List<ExposedMethodParam>();
            foreach (var arg in args)
            {
                var value = JsonConvert.SerializeObject(arg.Value, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                string typeFullName = null;
                if(arg.Value != null)
                {
                    typeFullName = arg.Value.GetType().FullName;
                }
                exposedMethodParams.Add(new ExposedMethodParam() { Name = arg.Key, Value = value, Type = typeFullName });
            }
            var grpcResult = await Backend.CallBusinessMethod(Name, method, exposedMethodParams);
            return await transformCallResponse(grpcResult);
        }


        public async Task<Protos.LoadResult> GetData(int? skip, int? take, string filter = "", string orderBy = "", bool includeCount = false, List<string> extraFields = null)
        {
            Protos.LoadResult result = new();
            try
            {
                result = await Backend.GetDataBusinessObj(Name, skip, take, filter, orderBy, includeCount, extraFields);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
        /*
        List<string> selectFields, int? skip, int? take, string filter = "", string orderBy = "", QueryFilterDelegate<T> queryFilter = null, bool includeCount = false
        */
        public async Task<Protos.LoadResult> GetUData(int? skip, int? take, string filter = "", string uFilter = "", string orderBy = "", bool includeCount = false, List<string> extraFields = null)
        {
            Protos.LoadResult result = new();
            try
            {
                result = await Backend.GetUData(Name, skip, take, filter, uFilter, orderBy, includeCount, extraFields);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

        public async Task<Protos.LoadResult> EntityFieldSearch(string searchText, string filters, int? top = null, string orderBy = "",
         List<string> extraFields = null)
        {
            Protos.LoadResult result = new();
            try
            {
                result = await Backend.EntityFieldSearch(Name, searchText, filters, top, orderBy, extraFields);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
    }
}