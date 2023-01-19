using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Entities.Enums;


namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Menu favorito
	/// </summary>

	[Index(nameof(RowidUser), nameof(RowidMenu), nameof(RowidMenuCustom), Name = "IX_e00063_1", IsUnique = true)]
	public partial class E00063_FavoritesMenu : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		public int RowidUser { get; set; }

		[ForeignKey("Menu")]
		public int? RowidMenu { get; set; }

		[ForeignKey("MenuCustom")]
		public int? RowidMenuCustom { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }

		[Required]
		public DateTime DateLastUse { get; set; }


		[SDKCheckRelationship]
		public virtual E00066_MenuCustom MenuCustom { get; set; }

		[SDKCheckRelationship]
		public virtual E00061_Menu Menu { get; set; }

	}
}