using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siesa.SDK.Protos;
using System.Reflection;
using Siesa.SDK.Shared.Business;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using Siesa.SDK.Shared.Json;
using Google.Protobuf;
using Siesa.SDK.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Siesa.SDK.GRPCServices
{
    public class SharedService : Siesa.SDK.Protos.Shared.SharedBase
    {
        private readonly ILogger<SharedService> _logger;
        private readonly IServiceProvider _provider;

        private IAuthenticationService _authenticationService;
        private IBackendRouterService _backendRouterService;
        
        public SharedService(ILogger<SharedService> logger, IServiceProvider provider, IAuthenticationService AuthenticationService, IBackendRouterService backendRouterService)
        {
            _logger = logger;
            _provider = provider;
            _authenticationService = AuthenticationService;
            _backendRouterService = backendRouterService;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new SDKContractResolver()
            };
        }

        private void SetCurrentUser(string token)
        {
            _authenticationService.SetToken(token);
        }

        public override Task<Protos.SetBackendServicesResponse> SetBackendServices(Protos.SetBackendServicesRequest request, ServerCallContext context)
        {
            SetCurrentUser(request.CurrentUserToken);
            var response = new Protos.SetBackendServicesResponse();

            Dictionary<string, BusinessModel> backendBusinesses = new Dictionary<string, BusinessModel>();

            foreach (var business in request.Businesses)
            {
               backendBusinesses.Add(business.Name, business);
            }
            _backendRouterService.SetBackendBusinesses(backendBusinesses);
            return Task.FromResult(response);
        }

    }
}
