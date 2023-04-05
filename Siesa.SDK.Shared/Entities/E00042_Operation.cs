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
	/// Operación
	/// </summary>

	[Index(nameof(RowidFeature), nameof(RowidAction), Name = "IX_e00042_1", IsUnique = true)]
	[Index(nameof(RowidFeature), nameof(Index), Name = "IX_e00042_2", IsUnique = true)]
	public partial class E00042_Operation : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Feature")]
		[SDKRequired]
		public int RowidFeature { get; set; }

		[ForeignKey("Action")]
		[SDKRequired]
		public int RowidAction { get; set; }

		[SDKRequired]
		public byte Index { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00040_Feature Feature { get; set; }

		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00041_Action Action { get; set; }

	}
}