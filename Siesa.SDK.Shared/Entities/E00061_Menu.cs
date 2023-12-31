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
	/// Menú
	/// </summary>

	[Index(nameof(RowidMenuParent), nameof(RowidResource), nameof(RowidFeature), nameof(Order), Name = "IX_e00061_1", IsUnique = true)]
	public partial class E00061_Menu : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("MenuParent")]
		public int? RowidMenuParent { get; set; }

		[ForeignKey("Resource")]
		public int? RowidResource { get; set; }

		[ForeignKey("Feature")]
		public int? RowidFeature { get; set; }

		[SDKRequired]
		public byte Order { get; set; }

		[SDKRequired]
		public EnumMenuType Type { get; set; }

		[SDKRequired]
		public byte Level { get; set; }

		public int? RowidImage { get; set; }

		[SDKStringLength(250)]
		public string? Url { get; set; }

		[SDKStringLength(250)]
		public string? IconClass { get; set; }


		[SDKCheckRelationship]
		public virtual E00020_Resource Resource { get; set; }

		[SDKCheckRelationship]
		public virtual E00040_Feature Feature { get; set; }

		[SDKCheckRelationship]
		public virtual E00061_Menu MenuParent { get; set; }

	}
}