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
	/// Relación entre portal y usuarios externos
	/// </summary>

	[Index(nameof(RowidPortal), nameof(RowidExternalUser), Name = "IX_e00518_1", IsUnique = true)]
	public partial class E00518_PortalUser : BaseSDK<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Portal")]
		[SDKRequired]
		public int RowidPortal { get; set; }

		[ForeignKey("ExternalUser")]
		[SDKRequired]
		public int RowidExternalUser { get; set; }

		[SDKRequired]
		public long RowidMainRecord { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00517_Portal Portal { get; set; }

		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00510_ExternalUser ExternalUser { get; set; }

	}
}