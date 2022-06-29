using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;


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
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("SuiteCustom")]
		[Required]
		public short RowidSuiteCustom { get; set; }

		[ForeignKey("MenuCustom")]
		public int? RowidMenuCustom { get; set; }

		[ForeignKey("Menu")]
		public int? RowidMenu { get; set; }

		[Required]
		public byte Order { get; set; }

		[Required]
		public bool IsPrivate { get; set; }


		public virtual E00066_MenuCustom MenuCustom { get; set; }

		[Required]
		public virtual E00065_SuiteCustom SuiteCustom { get; set; }

		public virtual E00061_Menu Menu { get; set; }

	}
}