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
	/// Tabla de módulos
	/// </summary>

	[Index(nameof(Id), nameof(LicenseType), Name = "IX_e00010_1", IsUnique = true)]
	public class E00010_Modules
	{
		[Key]
		[Required]
		public  int Rowid { get; set; }

		[Required]
		[StringLength(100)]
		public  string Id { get; set; }

		[RegularExpression(@"1|2|3")]
		[Required]
		public  byte LicenseType { get; set; } = 1;


	}
}