using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebDesigner_Blazor.Services;

namespace WebDesigner_Blazor.Controllers
{
	[Route("api/datasets")]
	public class DataSetsController : Controller
	{
		[HttpGet("{id}/content")]
		public ActionResult GetDataSetContent([FromRoute] string id)
		{
			if (string.IsNullOrWhiteSpace(id)) return BadRequest();
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
			var dataSet = id;
			
			// x.DataSource = new(){
			// 	Name = "Person",
			// 	ConnectionProperties = new(){
			// 		DataProvider = "JSON",
			// 		ConnectString = "jsondata={\"Data\":[{\"Id\":1,\"FirstName\":\"Nancy\",\"LastName\":\"Davolio\",\"Title\":\"Sales Representative\"},{\"Id\":2,\"FirstName\":\"Andrew\",\"LastName\":\"Fuller\",\"Title\":\"Vice President, Sales\"},{\"Id\":3,\"FirstName\":\"Janet\",\"LastName\":\"Leverling\",\"Title\":\"Sales Representative\"},{\"Id\":4,\"FirstName\":\"Margaret\",\"LastName\":\"Peacock\",\"Title\":\"Sales Representative\"},{\"Id\":5,\"FirstName\":\"Steven\",\"LastName\":\"Buchanan\",\"Title\":\"Sales Manager\"},{\"Id\":6,\"FirstName\":\"Michael\",\"LastName\":\"Suyama\",\"Title\":\"Sales Representative\"},{\"Id\":7,\"FirstName\":\"Robert\",\"LastName\":\"King\",\"Title\":\"Sales Representative\"},{\"Id\":8,\"FirstName\":\"Laura\",\"LastName\":\"Callahan\",\"Title\":\"Inside Sales Coordinator\"},{\"Id\":9,\"FirstName\":\"Anne\",\"LastName\":\"Dodsworth\",\"Title\":\"Sales Representative\"}]};schemadata={\"$id\":\"http://example.com/example.json\",\"type\":\"object\",\"definitions\":{},\"$schema\":\"http://json-schema.org/draft-07/schema#\",\"properties\":{\"Data\":{\"$id\":\"/properties/Data\",\"type\":\"array\",\"items\":{\"$id\":\"/properties/Data/items\",\"type\":\"object\",\"properties\":{\"Id\":{\"$id\":\"/properties/Data/items/properties/Id\",\"type\":\"integer\",\"title\":\"The Id Schema\",\"default\":0,\"examples\":[123]},\"FirstName\":{\"$id\":\"/properties/Data/items/properties/FirstName\",\"type\":\"string\",\"title\":\"The FirstName Schema\",\"default\":0,\"examples\":[\"abc\"]},\"LastName\":{\"$id\":\"/properties/Data/items/properties/LastName\",\"type\":\"string\",\"title\":\"The LastName Schema\",\"default\":0,\"examples\":[\"abc\"]},\"Title\":{\"$id\":\"/properties/Data/items/properties/Title\",\"type\":\"string\",\"title\":\"The Title Schema\",\"default\":0,\"examples\":[\"abc\"]}}}}}}",
			// 	},
			// };

			return Json(dataSet);
		}

		[HttpGet("list")]
		public ActionResult GetDataSetsList()
		{
			List<DataSetList> DataSetPerson = new List<DataSetList>()
			{
				new DataSetList() { Id = typeof(Person).FullName, Name = "Personas" },
				new DataSetList() { Id = typeof(Animals).FullName, Name = "Animales" },
				new DataSetList() { Id = typeof(Vehiculos).FullName, Name = "Vehiculos" },
			};

			// var dataSetsList = dataSetsService.GetDataSetsList();
			return Json(DataSetPerson);
		}

		public class Person
		{
			public string Name { get; set; }
			public string SubName { get; set; }
		}

		public class Animals
		{
			public string AnimalName { get; set; }
			public string AnimalSubName { get; set; }
		}

		public class Vehiculos
		{
			public string VehiculoName { get; set; }
			public string VehiculoSubName { get; set; }
		}

		public class DataSetList
		{
			public dynamic Id { get; set; }
			public string Name { get; set; }
		}
	}
}
