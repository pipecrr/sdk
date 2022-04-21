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
	[Index(nameof(RowidSuiteCustom), nameof(RowidMenuCustom), nameof(RowidMenu), nameof(Order), nameof(IsPrivate), Name = "IX_e00067_1", IsUnique = true)]
	public class E00067_SuiteMenuCustom : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		public short RowidSuiteCustom { get; set; }

		public int? RowidMenuCustom { get; set; }

		public int? RowidMenu { get; set; }

		[Required]
		public byte Order { get; set; }

		[Required]
		public bool IsPrivate { get; set; }


	}
}