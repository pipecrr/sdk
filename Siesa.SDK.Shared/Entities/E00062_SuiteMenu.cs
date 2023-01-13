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
	/// Suite Menu
	/// </summary>

	[Index(nameof(RowidSuite), nameof(RowidMenu), Name = "IX_e00062_1", IsUnique = true)]
	public partial class E00062_SuiteMenu : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Suite")]
		[Required]
		public short RowidSuite { get; set; }

		[ForeignKey("Menu")]
		[Required]
		public int RowidMenu { get; set; }

		[Required]
		public byte Order { get; set; }


		[SDKCheckRelationship]
		[Required]
		public virtual E00060_Suite Suite { get; set; }

		[SDKCheckRelationship]
		[Required]
		public virtual E00061_Menu Menu { get; set; }

	}
}