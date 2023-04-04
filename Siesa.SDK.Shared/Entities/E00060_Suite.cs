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
	/// Suite
	/// </summary>

	[Index(nameof(RowidResource), Name = "IX_e00060_1", IsUnique = true)]
	public partial class E00060_Suite : BaseSDK<short>
	{
		[SDKIdentity]
		[Key]
		[SDKRequired]
		public override short Rowid { get; set; }

		[ForeignKey("Resource")]
		[SDKRequired]
		public int RowidResource { get; set; }

		[SDKRequired]
		public byte Order { get; set; }

		public int? RowidImage { get; set; }

		[SDKStringLength(7)]
		public string? HexColor { get; set; }

		[SDKStringLength(250)]
		public string? IconClass { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00020_Resource Resource { get; set; }

	}
}