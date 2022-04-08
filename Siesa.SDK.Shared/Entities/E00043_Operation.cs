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
	/// Operaciones a realizar por programa
	/// </summary>

	[Index(nameof(RowidFeature), nameof(RowidAction), nameof(Index), Name = "IX_e00043_1")]
	public class E00043_Operation
	{
		[Key]
		[Required]
		public int Rowid { get; set; }

		[ForeignKey("Feature")]
		[Required]
		public int RowidFeature { get; set; }

		[ForeignKey("Action")]
		[Required]
		public int RowidAction { get; set; }

		[Required]
		public short Index { get; set; } = 0;


		[Required]
		public E00040_Feature Feature { get; set; }

		[Required]
		public E00042_Action Action { get; set; }

	}
}