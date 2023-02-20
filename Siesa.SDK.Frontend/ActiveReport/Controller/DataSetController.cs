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
using Newtonsoft.Json;


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
        public async Task<dynamic> GetDataSetSchema([FromBody]object data)
        {
            if (data == null) return BadRequest();

            var DataSetModel = JsonConvert.DeserializeObject<DataSetModel>(data.ToString());

            var DataSet = DataSetModel.DataSet;
            var DataSource = DataSetModel.DataSource;

            Type businessType = Siesa.SDK.Shared.Utilities.Utilities.SearchType(DataSetModel.DataSet.Query.CommandText, true);

			var entityType = businessType.GetProperty("BaseObj").PropertyType;

			List<dynamic> DataSetEntity = new List<dynamic>();
			
			if (entityType != null)
			{
				var EntityFields = entityType.GetProperties();

				foreach (var EntityField in EntityFields)
				{
					DataSetEntity.Add(new
					{
						Name = EntityField.Name,
						DataField = EntityField.Name,
						DataType = EntityField.PropertyType.Name,
						Aggregate = "none"
					});
				}
            }

           /* SchemaResult schemaResult = new SchemaResult()
            {
                fields = DataSetEntity.Select(f => new Field()
                {
                    name = f.Name,
                    type = f.DataType,
                }).ToArray(),
                parameters = new Parameter[0]
            };*/
            
            SchemaResult schemaResult = new SchemaResult()
            {
                fields = DataSetEntity.Select(async f => new Field()
                {
                    name = await _resourceManager.GetResource($"{entityType.Name}.{f.Name}",1),
                    type = f.DataType,
                }).ToList().Select(f => f.Result).ToArray(),
                parameters = new Parameter[0]
            };
            
            /*{
                fields = DataSet.Fields.Select(f => new Field()
                {
                    name = f.Name,
                    type = f.DataType,
                }).ToArray(),
                parameters = new Parameter[0]
            };*/
    
            return Json(schemaResult);
        }
        public class SchemaResult {
            public Field[] fields { get; set; }
            public Parameter[] parameters { get; set; }
        }
        public class Field {
            public string name { get; set; }
            public string type { get; set; }
        }
    }
}