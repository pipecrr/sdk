using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siesa.SDK.Protos;
using Grpc.Net.Client;
using Grpc.Core;
using Siesa.SDK.Shared.Backend;
using Newtonsoft.Json;
using Siesa.SDK.Shared.Business;
using Google.Protobuf;
using System.Net.Http;
using System.Runtime.InteropServices;
using Siesa.SDK.Shared.Json;
using System.Reflection;
using System.IO;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend
{

    public class BusinessFrontendModel
    {
        private IAuthenticationService AuthenticationService { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Entity { get; set; }
        public string BackendName { get; set; }

        public void SetAuthenticationService(IAuthenticationService authenticationService)
        {
            AuthenticationService = authenticationService;
        }

        public BackendRegistry Backend {get { return BackendManager.Instance.GetBackend(BackendName, AuthenticationService); } }

        public dynamic Save(dynamic obj)
        {
            return Backend.SaveBusiness(Name, obj);
        }

        public dynamic ValidateAndSave(dynamic obj)
        {
            return Backend.ValidateAndSaveBusiness(Name, obj);
        }

        public dynamic Delete(Int64 id)
        {
            return Backend.DeleteBusinessObj(Name, id);
        }

        public dynamic Get(Int64 id)
        {
            return Backend.GetBusinessObj(Name, id);
        }

        private async Task<ActionResult<dynamic>> transformCallResponse(ExposedMethodResponse grpcResult){
            if(grpcResult == null){
                return new ActionResult<dynamic>(){
                    Success = false,
                    Errors = new List<string>(){
                        "No response from backend"
                    }
                };
            }
            var response = new ActionResult<dynamic>() { Success = grpcResult.Success, Errors = grpcResult.Errors };
            if(response.Success){
                Type t = null;
                if(!string.IsNullOrEmpty(grpcResult.DataType)){
                    t = Utils.Utils.SearchType(grpcResult.DataType, true);
                }
                //response.Data = JsonConvert.DeserializeObject(grpcResult.Data, t);
   
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
                exposedMethodParams.Add(new ExposedMethodParam() { Name = "", Value = value, Type = arg.GetType().FullName });
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
                exposedMethodParams.Add(new ExposedMethodParam() { Name = arg.Key, Value = value, Type = arg.Value.GetType().FullName });
            }
            var grpcResult = await Backend.CallBusinessMethod(Name, method, exposedMethodParams);
            return await transformCallResponse(grpcResult);
        }


        public async Task<Protos.LoadResult> GetData(int? skip, int? take, string filter = "", string orderBy = "")
        {
            Protos.LoadResult result = new();
            try
            {
                result = await Backend.GetDataBusinessObj(Name, skip, take, filter, orderBy);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

         public async Task<Protos.LoadResult> EntityFieldSearch(string searchText, string filters)
        {
            Protos.LoadResult result = new();
            try
            {
                result = await Backend.EntityFieldSearch(Name, searchText, filters);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }
    }

    public class BusinessManagerFrontend
    {
        private static BusinessManagerFrontend _instance;
        public Dictionary<string, BusinessFrontendModel> Businesses { get; set; }
        private BusinessManagerFrontend()
        {
        }
        public static BusinessManagerFrontend Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BusinessManagerFrontend();
                    _instance.Businesses = new Dictionary<string, BusinessFrontendModel>();
                }
                return _instance;
            }
        }

        public void AddBusiness(BusinessModel business, string backendName)
        {
            if (!Businesses.ContainsKey(business.Name)) { 
                var businessFrontendModel = new BusinessFrontendModel();
                businessFrontendModel.Name = business.Name;
                businessFrontendModel.Namespace = business.Namespace;
                businessFrontendModel.BackendName = backendName;
                Businesses.Add(business.Name, businessFrontendModel);
            }
        }

        public string GetViewdef(string businessName, string viewName) {
            var business = Businesses[businessName];
            if (business == null) {
                return null;
            }
            var asm = Utils.Utils.SearchAssemblyByType(business.Namespace + "." + business.Name);
            if (asm == null)
            {
                return null;
            }
            return Utils.Utils.ReadAssemblyResource(asm, business.Name + ".Viewdefs."+ viewName + ".json");
        }

        //get business
        public BusinessFrontendModel GetBusiness(string businessName, IAuthenticationService authenticationService)
        {
            var business = Businesses[businessName];
            if (business == null)
            {
                return null;
            }
            business.SetAuthenticationService(authenticationService);
            return business;
        }
    }
}
