using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Siesa.SDK.Frontend.Services;
using Siesa.SDK.Protos;
using WebDesigner_Blazor.Services;
using Siesa.SDK.Shared.Services;
using Siesa.SDK.Shared.Utilities;
using System.Linq;

namespace SDK.Frontend.ReportDesigner.Controllers
{
	[Route("api/datasets")]
	public class DataSetsController : Controller
	{
		private IBackendRouterService _backendRouterService;

		public DataSetsController(IBackendRouterService backendRouterService)
        {
            _backendRouterService = backendRouterService;
        }

		[HttpGet("{id}/content")]
		public ActionResult GetDataSetContent([FromRoute] string id)
		{
			if (string.IsNullOrWhiteSpace(id)) return BadRequest();

			Type EntityType = Utilities.SearchType(id, true);

			var EntityFields = EntityType.GetProperties();

			List<dynamic> DataSetEntity = new List<dynamic>();

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
			var NameDataSet = EntityType.Name.Split('_').Last();

			 var _DataSetModel  = new DataSetModel(){
			 	DataSet = new(){
			 		Name = NameDataSet,
			 		Fields = DataSetEntity.Select(x => new Field() { Name = x.Name, DataField = x.DataField, DataType = x.DataType, Aggregate = x.Aggregate }).ToArray()
					,
			 		Query = new(){
			 			CommandText = "x",
			 			DataSourceName = "x"
			 		}
			 	},
			 	DataSource = new(){
			 		Name = "Entities",
			 		ConnectionProperties = new(){
			 			DataProvider = "x",
			 			ConnectString = "x",
			 		}
			 	}
			 };
				
			return Json(_DataSetModel);
			
			// var _DataSetModel  = new DataSetModel(){
			// 	DataSet = new(){
			// 		Name = "Prueba",
			// 		Fields = new[]{
			// 			new Field() { Name = "Name", DataField = "Name", DataType = "string", Aggregate = "none" },
			// 		},
			// 		Query = new(){
			// 			CommandText = "x",
			// 			DataSourceName = "x"
			// 		}
			// 	},
			// 	DataSource = new(){
			// 		Name = "x",
			// 		ConnectionProperties = new(){
			// 			DataProvider = "x",
			// 			ConnectString = "x",
			// 		}
			// 	}
			// };

			
		
			// var dataSet = (string)dataSetsService.GetDataSet(id);
			// var x  = new DataSetModel();
			// x.DataSet = new(){
			// 	Name = "Prueba",
			// 	Fields = new[]{

			// 		new Field() { Name = "Name", DataField = "Name", DataType = "string", Aggregate = "none" },
			// 		new Field() { Name = "SubName", DataField = "SubName", DataType = "string", Aggregate = "none" },
			// 	},
			// 	Query = new(){
			// 		CommandText = "x",
			// 		DataSourceName = "Person",
			// 	},
			// };
			//var dataSet = id;
			
			// x.DataSource = new(){
			// 	Name = "Person",
			// 	ConnectionProperties = new(){
			// 		DataProvider = "JSON",
			// 		ConnectString = "jsondata={\"Data\":[{\"Id\":1,\"FirstName\":\"Nancy\",\"LastName\":\"Davolio\",\"Title\":\"Sales Representative\"},{\"Id\":2,\"FirstName\":\"Andrew\",\"LastName\":\"Fuller\",\"Title\":\"Vice President, Sales\"},{\"Id\":3,\"FirstName\":\"Janet\",\"LastName\":\"Leverling\",\"Title\":\"Sales Representative\"},{\"Id\":4,\"FirstName\":\"Margaret\",\"LastName\":\"Peacock\",\"Title\":\"Sales Representative\"},{\"Id\":5,\"FirstName\":\"Steven\",\"LastName\":\"Buchanan\",\"Title\":\"Sales Manager\"},{\"Id\":6,\"FirstName\":\"Michael\",\"LastName\":\"Suyama\",\"Title\":\"Sales Representative\"},{\"Id\":7,\"FirstName\":\"Robert\",\"LastName\":\"King\",\"Title\":\"Sales Representative\"},{\"Id\":8,\"FirstName\":\"Laura\",\"LastName\":\"Callahan\",\"Title\":\"Inside Sales Coordinator\"},{\"Id\":9,\"FirstName\":\"Anne\",\"LastName\":\"Dodsworth\",\"Title\":\"Sales Representative\"}]};schemadata={\"$id\":\"http://example.com/example.json\",\"type\":\"object\",\"definitions\":{},\"$schema\":\"http://json-schema.org/draft-07/schema#\",\"properties\":{\"Data\":{\"$id\":\"/properties/Data\",\"type\":\"array\",\"items\":{\"$id\":\"/properties/Data/items\",\"type\":\"object\",\"properties\":{\"Id\":{\"$id\":\"/properties/Data/items/properties/Id\",\"type\":\"integer\",\"title\":\"The Id Schema\",\"default\":0,\"examples\":[123]},\"FirstName\":{\"$id\":\"/properties/Data/items/properties/FirstName\",\"type\":\"string\",\"title\":\"The FirstName Schema\",\"default\":0,\"examples\":[\"abc\"]},\"LastName\":{\"$id\":\"/properties/Data/items/properties/LastName\",\"type\":\"string\",\"title\":\"The LastName Schema\",\"default\":0,\"examples\":[\"abc\"]},\"Title\":{\"$id\":\"/properties/Data/items/properties/Title\",\"type\":\"string\",\"title\":\"The Title Schema\",\"default\":0,\"examples\":[\"abc\"]}}}}}}",
			// 	},
			// };


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

				DataSetEntity.Add(new { Id = entityType.FullName, Name = entityName });
			}
			string x = "x";

			/*List<DataSetList> DataSetPerson = new List<DataSetList>()
			{
				new DataSetList() { Id = typeof(Person).FullName, Name = "Personas" },
				new DataSetList() { Id = typeof(Animals).FullName, Name = "Animales" },
				new DataSetList() { Id = typeof(Vehiculos).FullName, Name = "Vehiculos" },
			};*/

			// var dataSetsList = dataSetsService.GetDataSetsList();
			return Json(DataSetEntity);
		}

		// public class Person
		// {
		// 	public string Name { get; set; }
		// 	public string SubName { get; set; }
		// }

		// public class Animals
		// {
		// 	public string AnimalName { get; set; }
		// 	public string AnimalSubName { get; set; }
		// }

		// public class Vehiculos
		// {
		// 	public string VehiculoName { get; set; }
		// 	public string VehiculoSubName { get; set; }
		// }

		// public class DataSetList
		// {
		// 	public dynamic Id { get; set; }
		// 	public string Name { get; set; }
		// }
	}
}
