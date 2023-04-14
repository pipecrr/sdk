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
	/// Menú  personalizado 
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidMenuCustomParent), nameof(Id), nameof(RowidFeature), nameof(Order), Name = "IX_e00066_1", IsUnique = true)]
	public partial class E00066_MenuCustom : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("MenuCustomParent")]
		public int? RowidMenuCustomParent { get; set; }

		[SDKStringLength(500)]
		public string? Id { get; set; }

		[ForeignKey("Feature")]
		public int? RowidFeature { get; set; }

		[SDKRequired]
		public byte Order { get; set; }

		[SDKRequired]
		public byte Type { get; set; }

		[SDKRequired]
		public byte Level { get; set; }

		public int? RowidImage { get; set; }

		[SDKRequired]
		public bool IsPrivate { get; set; }


		[SDKCheckRelationship]
		public virtual E00040_Feature Feature { get; set; }

		[SDKCheckRelationship]
		public virtual E00066_MenuCustom MenuCustomParent { get; set; }

	}
}