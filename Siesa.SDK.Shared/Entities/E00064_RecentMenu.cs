using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.Global.Enums;



namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Menú reciente
	/// </summary>

	[Index(nameof(RowidUser), nameof(RowidMenu), nameof(RowidMenuCustom), Name = "IX_e00064_1", IsUnique = true)]
	public partial class E00064_RecentMenu : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		public int RowidUser { get; set; }

		[ForeignKey("Menu")]
		public int? RowidMenu { get; set; }

		[ForeignKey("MenuCustom")]
		public int? RowidMenuCustom { get; set; }

		[SDKRequired]
		[SDKStringLength(500)]
		public string Description { get; set; }

		[SDKRequired]
		public DateTime DateLastUse { get; set; }


		[SDKCheckRelationship]
		public virtual E00066_MenuCustom MenuCustom { get; set; }

		[SDKCheckRelationship]
		public virtual E00061_Menu Menu { get; set; }

	}
}