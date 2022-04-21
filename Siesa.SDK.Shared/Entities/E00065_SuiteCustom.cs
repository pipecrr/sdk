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
	/// Suite personalizada
	/// </summary>
	[SDKLogEntity]
	public class E00065_SuiteCustom : BaseAudit<short>
	{
		[Key]
		[Required]
		public override short Rowid { get; set; }

		[Required]
		[StringLength(500)]
		public string Id { get; set; }

		[Required]
		public byte Order { get; set; }

		[Required]
		public int Image { get; set; }

		[Required]
		public bool IsPrivate { get; set; }


	}
}