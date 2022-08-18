using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Backend;
using Siesa.SDK.Shared.Configurations;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
namespace Siesa.SDK.Frontend.Services
{
    
    public class BackendRouterService: BackendRouterServiceBase
    {
        public BackendRouterService(IOptions<ServiceConfiguration> serviceConfiguration) : base(serviceConfiguration)
        {
        }

        public override string GetViewdef(string businessName, string viewName)
        {
            var business = this.GetBackend(businessName);
            if (business == null) {
                return null;
            }
            var asm = Utilities.SearchAssemblyByType(business.Namespace + "." + business.Name);
            if (asm == null)
            {
                return null;
            }
            return Utilities.ReadAssemblyResource(asm, business.Name + ".Viewdefs."+ viewName + ".json");
        }
    }
}