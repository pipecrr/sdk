using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Siesa.SDK.Frontend.Application;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Backend;
using Siesa.SDK.Shared.Configurations;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
namespace Siesa.SDK.Frontend.Services
{
    
    public class BackendRouterService: BackendRouterServiceBase
    {
        private readonly IViewdefManager _viewdefManager;
        public BackendRouterService(IOptions<ServiceConfiguration> serviceConfiguration, IViewdefManager viewdefManager) : base(serviceConfiguration)
        {
            _viewdefManager = viewdefManager;
        }

        public override string GetViewdef(string businessName, string viewName)
        {
            if (string.IsNullOrEmpty(businessName) || string.IsNullOrEmpty(viewName))
            {
                return null;
            }

            if(_viewdefManager == null)
            {
                return null;
            }

            return _viewdefManager.GetViewdef(businessName, viewName);
        }
    }
}