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
	/// Relación de programas que pertenecen a un módulo 
	/// </summary>

	[Index(nameof(RowidModule), nameof(RowidFeature), Name = "IX_e00041_1", IsUnique = true)]
	public class E00041_ModuleFeature
	{
		[Key]
		[Required]
		public int Rowid { get; set; }

		[ForeignKey("Module")]
		[Required]
		public short RowidModule { get; set; }

		[ForeignKey("Feature")]
		[Required]
		public int RowidFeature { get; set; }


		[Required]
		public E00010_Module Module { get; set; }

		[Required]
		public E00040_Feature Feature { get; set; }

	}
}