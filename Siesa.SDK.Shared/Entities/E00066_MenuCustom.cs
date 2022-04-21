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
	/// Menú  personalizado 
	/// </summary>
	[SDKLogEntity]
	public class E00066_MenuCustom : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		public int? RowidMenuCustomParent { get; set; }

		[StringLength(500)]
		public string? Id { get; set; }

		public int? RowidFeature { get; set; }

		[Required]
		public byte Order { get; set; }

		[Required]
		public byte Type { get; set; }

		[Required]
		public byte Level { get; set; }

		[Required]
		public int Image { get; set; }

		[Required]
		public bool IsPrivate { get; set; }


	}
}