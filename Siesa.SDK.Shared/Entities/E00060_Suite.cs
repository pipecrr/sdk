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
	/// Suite
	/// </summary>

	[Index(nameof(RowidResource), nameof(Order), nameof(RowidImage), Name = "IX_e00060_1", IsUnique = true)]
	public class E00060_Suite : BaseSDK<short>
	{
		[Key]
		[Required]
		public override short Rowid { get; set; }

		[ForeignKey("Resource")]
		[Required]
		public int RowidResource { get; set; }

		[Required]
		public byte Order { get; set; }

		public int? RowidImage { get; set; }


		[Required]
		public E00020_Resource Resource { get; set; }

	}
}