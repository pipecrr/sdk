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

namespace Siesa.SDK.Frontend
{

    public class BusinessFrontendModel
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Entity { get; set; }
        public string BackendName { get; set; }

        public BackendRegistry Backend {get { return BackendManager.Instance.GetBackend(BackendName); } }

        public dynamic Save(dynamic obj)
        {
            return Backend.SaveBusiness(Name, obj);
        }

        public dynamic ValidateAndSave(dynamic obj)
        {
            return Backend.ValidateAndSaveBusiness(Name, obj);
        }

        public dynamic Delete(int id)
        {
            return Backend.DeleteBusinessObj(Name, id);
        }

        public dynamic Get(int id)
        {
            return Backend.GetBusinessObj(Name, id);
        }

        private ActionResult<dynamic> transformCallResponse(ExposedMethodResponse grpcResult){
            var response = new ActionResult<dynamic>() { Success = grpcResult.Success, Errors = grpcResult.Errors };
            if(response.Success){
                Type t = null;
                if(!string.IsNullOrEmpty(grpcResult.DataType)){
                    t = Utils.Utils.SearchType(grpcResult.DataType, true);
                }
                response.Data = JsonConvert.DeserializeObject(grpcResult.Data, t);
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
            return transformCallResponse(grpcResult);
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
            return transformCallResponse(grpcResult);
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

         public async Task<Protos.LoadResult> EntityFieldSearch(string searchText)
        {
            Protos.LoadResult result = new();
            try
            {
                result = await Backend.EntityFieldSearch(Name, searchText);
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
        public BusinessFrontendModel GetBusiness(string businessName)
        {
            var business = Businesses[businessName];
            if (business == null)
            {
                return null;
            }
            return business;
        }
    }
}
