using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siesa.SDK.Protos;
using Grpc.Net.Client;
using Grpc.Core;
using Siesa.SDK.Shared.Backend;

namespace Siesa.SDK.Frontend
{

    public class BusinessFrontendModel
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public string Entity { get; set; }
        public string BackendName { get; set; }

        public dynamic Save(dynamic obj)
        {
            BackendRegistry backend = BackendManager.Instance.GetBackend(BackendName);
            return backend.SaveBusiness(Name, obj);
        }

        public dynamic ValidateAndSave(dynamic obj)
        {
            BackendRegistry backend = BackendManager.Instance.GetBackend(BackendName);
            return backend.ValidateAndSaveBusiness(Name, obj);
        }

        public dynamic Delete(int id)
        {
            BackendRegistry backend = BackendManager.Instance.GetBackend(BackendName);
            return backend.DeleteBusinessObj(Name, id);
        }

        public dynamic Get(int id)
        {
            BackendRegistry backend = BackendManager.Instance.GetBackend(BackendName);
            return backend.GetBusinessObj(Name, id);
        }

        public async Task<LoadResult> GetData(int? skip, int? take, string filter = "", string orderBy = "")
        {
            BackendRegistry backend = BackendManager.Instance.GetBackend(BackendName);
            LoadResult result = new();
            try
            {
                result = await backend.GetDataBusinessObj(Name, skip, take, filter, orderBy);
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result;
        }

         public async Task<LoadResult> EntityFieldSearch(string searchText)
        {
            BackendRegistry backend = BackendManager.Instance.GetBackend(BackendName);
            LoadResult result = new();
            try
            {
                result = await backend.EntityFieldSearch(Name, searchText);
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
