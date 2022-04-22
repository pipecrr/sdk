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
	/// Menú
	/// </summary>

	[Index(nameof(RowidMenuParent), nameof(RowidResource), nameof(RowidFeature), nameof(Order), nameof(Type), nameof(Level), nameof(RowidImage), Name = "IX_e00061_1", IsUnique = true)]
	public class E00061_Menu : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("MenuParent")]
		public int? RowidMenuParent { get; set; }

		[ForeignKey("Resource")]
		public int? RowidResource { get; set; }

		[ForeignKey("Feature")]
		public int? RowidFeature { get; set; }

		[Required]
		public byte Order { get; set; }

		[Required]
		public byte Type { get; set; }

		[Required]
		public byte Level { get; set; }

		public int? RowidImage { get; set; }


		public E00061_Menu MenuParent { get; set; }

		public E00020_Resource Resource { get; set; }

		public E00040_Feature Feature { get; set; }

	}
}