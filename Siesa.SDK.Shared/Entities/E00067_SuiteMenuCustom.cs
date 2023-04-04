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
	/// Suite Menú personalizado 
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidSuiteCustom), nameof(RowidMenuCustom), nameof(RowidMenu), Name = "IX_e00067_1", IsUnique = true)]
	public partial class E00067_SuiteMenuCustom : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("SuiteCustom")]
		[SDKRequired]
		public short RowidSuiteCustom { get; set; }

		[ForeignKey("MenuCustom")]
		public int? RowidMenuCustom { get; set; }

		[ForeignKey("Menu")]
		public int? RowidMenu { get; set; }

		[SDKRequired]
		public byte Order { get; set; }

		[SDKRequired]
		public bool IsPrivate { get; set; }


		[SDKCheckRelationship]
		public virtual E00066_MenuCustom MenuCustom { get; set; }

		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00065_SuiteCustom SuiteCustom { get; set; }

		[SDKCheckRelationship]
		public virtual E00061_Menu Menu { get; set; }

	}
}