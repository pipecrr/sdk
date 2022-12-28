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
	[Route("api/datasets")]
	public class DataSetsController : Controller
	{
		private IBackendRouterService _backendRouterService;
		public IResourceManager _resourceManager;
		IAuthenticationService _authenticationService;

		public DataSetsController(IBackendRouterService backendRouterService, IResourceManager resourceManager, IAuthenticationService authenticationService)
        {
            _backendRouterService = backendRouterService;
			_resourceManager = resourceManager;
			_authenticationService = authenticationService;
        }

		[HttpGet("{id}/content")]
		public async Task<dynamic> GetDataSetContent([FromRoute] string id)
		{
			if (string.IsNullOrWhiteSpace(id)) return BadRequest();

			Type EntityType = Utilities.SearchType(id, true);

			List<dynamic> DataSetEntity = new List<dynamic>();
			
			if (true)//EntityType != null
			{
				
				var EntityFields = EntityType.GetProperties();

				
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

			var NameDataSet =  await _resourceManager.GetResource($"{EntityType.Name}.Singular", 1);


			 var _DataSetModel  = new DataSetModel(){
			 	DataSet = new(){
			 		Name = NameDataSet,
			 		Fields = await Task.WhenAll(DataSetEntity.Select(async x => new Field() 
					{ 
					Name = await _resourceManager.GetResource($"{EntityType.Name}.{x.Name}", 1), 
					DataField = x.DataField, 
					DataType = x.DataType, 
					Aggregate = x.Aggregate
					}))
					,
			 		 Query = new(){
			 		 	CommandText = "BLCity",
			 		 	DataSourceName = "BLCity"
			 		 }
			 	},
			 	DataSource = new(){
			 		Name = "BLCity",
			 		ConnectionProperties = new(){
			 			DataProvider = "Siesa.SDK.Business",
			 			ConnectString = "BLCity",
			 		}
			 	}
			 };
				
			return Json(_DataSetModel);
			
		}

		[HttpGet("list")]
		public ActionResult GetDataSetsList()
		{
			List<BusinessModel> bussines = _backendRouterService.GetBusinessModelList();

			List<dynamic> DataSetEntity = new List<dynamic>();

			foreach (var bsModel in bussines)
			{
				Type businessType = Utilities.SearchType(bsModel.Namespace + "." + bsModel.Name, true);
				if(businessType == null)
				{
					continue;
				}

				var entityType = businessType.GetProperty("BaseObj").PropertyType;
				string entityName = entityType.Name;

				if (!DataSetEntity.Where(x => x.Id == entityType.FullName).Any())
				{
					DataSetEntity.Add(new { Id = entityType.FullName, Name = entityName });
				}

				var BusinessMethods = businessType.GetMethods().Where(x => x.GetCustomAttributes(typeof(SDKDataSourceReport), false).Length > 0);

				if(BusinessMethods != null && BusinessMethods.Count() != 0)
				{
					foreach (var Method in BusinessMethods)
					{
						if (Method.ReturnType.IsGenericType)
						{
							var ReturnValueType = Method.ReturnType.GetGenericArguments()[0];
							DataSetEntity.Add(new { Id = ReturnValueType.FullName, Name = ReturnValueType.Name });
						}
					}
				}
			}

			return Json(DataSetEntity);
		}
	}
}
