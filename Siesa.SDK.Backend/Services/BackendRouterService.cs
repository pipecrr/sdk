using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Backend;
using Siesa.SDK.Shared.Configurations;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Backend.Services
{
    
    public class BackendRouterService: BackendRouterServiceBase
    {
        public BackendRouterService(IOptions<ServiceConfiguration> serviceConfiguration) : base(serviceConfiguration)
        {
        }
    }
}