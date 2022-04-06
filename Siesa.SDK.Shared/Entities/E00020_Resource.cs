using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;


namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Tabla con los ID de los recursos
	/// </summary>

	[Index(nameof(Id), Name = "IX_e00020_1", IsUnique = true)]
	public class E00020_Resource
	{
		[Key]
		[Required]
		public  int Rowid { get; set; }

		[Required]
		[StringLength(500)]
		public  string Id { get; set; }


	}
}