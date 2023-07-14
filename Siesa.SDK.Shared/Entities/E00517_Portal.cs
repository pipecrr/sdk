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
	/// Portal
	/// </summary>

	[Index(nameof(Id), Name = "IX_e00517_1", IsUnique = true)]
	public partial class E00517_Portal : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(20)]
		public string Id { get; set; }

		[ForeignKey("PortalType")]
		[SDKRequired]
		public int RowidPortalType { get; set; }

		[ForeignKey("InternalUser")]
		[SDKRequired]
		public int RowidInternalUser { get; set; }

		[SDKStringLength(10)]
		public string? IconClass { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00515_PortalType PortalType { get; set; }

		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00220_User InternalUser { get; set; }

	}
}