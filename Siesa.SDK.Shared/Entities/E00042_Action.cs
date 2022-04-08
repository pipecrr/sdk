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
	/// Acciones dentro del programa
	/// </summary>

	[Index(nameof(RowidResource), Name = "IX_e00042_1", IsUnique = true)]
	public class E00042_Action
	{
		[Key]
		[Required]
		public int Rowid { get; set; }

		[ForeignKey("Resource")]
		[Required]
		public int RowidResource { get; set; }


		[Required]
		public E00020_Resource Resource { get; set; }

	}
}