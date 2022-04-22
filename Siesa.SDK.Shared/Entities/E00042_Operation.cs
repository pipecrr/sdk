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
	/// Operación
	/// </summary>

	[Index(nameof(RowidFeature), nameof(RowidAction), nameof(Index), Name = "IX_e00042_1", IsUnique = true)]
	public class E00042_Operation : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Feature")]
		[Required]
		public int RowidFeature { get; set; }

		[ForeignKey("Action")]
		[Required]
		public int RowidAction { get; set; }

		[Required]
		public byte Index { get; set; }


		[Required]
		public E00040_Feature Feature { get; set; }

		[Required]
		public E00041_Action Action { get; set; }

	}
}