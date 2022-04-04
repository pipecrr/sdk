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
	/// Tabla de Módulos del sistema
	/// </summary>

	[Index(nameof(Id), nameof(RowidResource), nameof(LicenseType), Name = "IX_e00040_1", IsUnique = true)]
	public class E00040_Module
	{
		[Key]
		[Required]
		public  int Rowid { get; set; }

		[Required]
		public  byte Id { get; set; }

		[ForeignKey("Resource")]
		[Required]
		public  int RowidResource { get; set; }

		[Required]
		public  byte LicenseType { get; set; }


		[Required]
		public E00020_Resource Resource { get; set; }

	}
}