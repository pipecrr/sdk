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
	/// Suite Menu
	/// </summary>

	[Index(nameof(RowidSuite), nameof(RowidMenu), Name = "IX_e00062_1", IsUnique = true)]
	public partial class E00062_SuiteMenu : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Suite")]
		[SDKRequired]
		public short RowidSuite { get; set; }

		[ForeignKey("Menu")]
		[SDKRequired]
		public int RowidMenu { get; set; }

		[SDKRequired]
		public byte Order { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00060_Suite Suite { get; set; }

		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00061_Menu Menu { get; set; }

	}
}