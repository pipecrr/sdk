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
	/// Menú reciente
	/// </summary>

	[Index(nameof(RowidUser), nameof(RowidMenu), nameof(RowidMenuCustom), nameof(Description), nameof(DateLastUse), Name = "IX_e00064_1", IsUnique = true)]
	public class E00064_RecentMenu : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		public int RowidUser { get; set; }

		public int? RowidMenu { get; set; }

		public int? RowidMenuCustom { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }

		[Required]
		[MaxLength(7)]
		public DateTime DateLastUse { get; set; }


	}
}