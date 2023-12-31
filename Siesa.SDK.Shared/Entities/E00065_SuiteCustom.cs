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
	/// Suite personalizada
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(Id), Name = "IX_e00065_1", IsUnique = true)]
	public partial class E00065_SuiteCustom : BaseAudit<short>
	{
		[Key]
		[SDKRequired]
		public override short Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(500)]
		public string Id { get; set; }

		[SDKRequired]
		public byte Order { get; set; }

		public int? RowidImage { get; set; }

		[SDKRequired]
		public bool IsPrivate { get; set; }


	}
}