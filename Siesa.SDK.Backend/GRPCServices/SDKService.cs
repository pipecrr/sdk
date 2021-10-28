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

namespace Siesa.SDK.GRPCServices
{
    public class SDKService : Siesa.SDK.Protos.SDK.SDKBase
    {
        private readonly ILogger<SDKService> _logger;
        private readonly IServiceProvider _provider;
        public SDKService(ILogger<SDKService> logger, IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        public override Task<Protos.BusinessesResponse> GetBusinesses(Protos.GetBusinessesRequest request, ServerCallContext context){
            var response = new Protos.BusinessesResponse();
            response.Businesses.AddRange(BusinessManager.Instance.Businesses.Values);
            return Task.FromResult(response);
        }

        public override Task<Protos.BusinessObjResponse> GetBusinessObj(Protos.GetBusinessObjRequest request, ServerCallContext context)
        {
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = Activator.CreateInstance(businessType);
            businessObj.SetProvider(_provider);
            
            var response = new Protos.BusinessObjResponse();
            var result = businessObj.Get(request.Id);
            response.Response = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return Task.FromResult(response);
        }

        private Type FindType(string name)
        {
            Type businessType = null;
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (ass.FullName.StartsWith("System.") || ass.FullName.StartsWith("Microsoft.") || ass.FullName.StartsWith("mscorlib") || ass.FullName.StartsWith("netstandard") || ass.FullName.StartsWith("GRPC."))
                    continue;
                businessType = ass.GetType(name);
                if (businessType != null)
                    break;
            }
            return businessType;
        }

        public override Task<Protos.SaveBusinessObjResponse> SaveBusinessObj(Protos.SaveBusinessObjRequest request, ServerCallContext context)
        {
            var response = new Protos.SaveBusinessObjResponse();
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            //dynamic x = Activator.CreateInstance(businessType);
            //json deserialize using Newtonsoft.Json
            dynamic businessObj = Newtonsoft.Json.JsonConvert.DeserializeObject(request.Business, businessType);
            businessObj.SetProvider(_provider);
            response.Id = businessObj.Save();
            return Task.FromResult(response);
        }

        public override Task<Protos.DeleteBusinessObjResponse> DeleteBusinessObj(Protos.DeleteBusinessObjRequest request, ServerCallContext context)
        {
            BusinessModel businessRegistry = BusinessManager.Instance.GetBusiness(request.BusinessName);
            var businessType = FindType(businessRegistry.Namespace + "." + businessRegistry.Name);
            dynamic businessObj = Activator.CreateInstance(businessType);
            businessObj.SetProvider(_provider);

            var response = new Protos.DeleteBusinessObjResponse();
            var result = businessObj.Get(request.Id);
            businessObj.BaseObj = result;
            response.Id = businessObj.Delete();
            return Task.FromResult(response);
        }


    }
}
