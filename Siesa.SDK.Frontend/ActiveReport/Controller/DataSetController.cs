using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Siesa.SDK.Protos;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
using System.Linq;
using Siesa.SDK.Frontend.Application;
using System.Threading.Tasks;
using Siesa.SDK.Shared.DataAnnotations;


namespace Siesa.SDK.Frontend.Report.Controllers
{
	[Route("api/dataset")]
	public class DataSetController : Controller
	{

        private IBackendRouterService _backendRouterService;
		public IResourceManager _resourceManager;
		IAuthenticationService _authenticationService;

		public DataSetController(IBackendRouterService backendRouterService, IResourceManager resourceManager, IAuthenticationService authenticationService)
        {
            _backendRouterService = backendRouterService;
			_resourceManager = resourceManager;
			_authenticationService = authenticationService;
        }

        [HttpPost("schema")]
        public async Task<SchemaResult> GetDataSetSchema()
		{
            return new SchemaResult();
        }

        
    }


    public class SchemaResult {
        public Field[] fields { get; set; }
        public Parameter[] parameters { get; set; }
    }
}