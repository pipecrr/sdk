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
	/// Recursos personalizados
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidResourceDescription), nameof(Description), Name = "IX_e00023_1")]
	public class E00023_ResourceCustomDescription : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("ResourceDescription")]
		[Required]
		public  int RowidResourceDescription { get; set; }

		[Required]
		[StringLength(2000)]
		public  string Description { get; set; }


		[Required]
		public E00022_ResourceDescription ResourceDescription { get; set; }

	}
}