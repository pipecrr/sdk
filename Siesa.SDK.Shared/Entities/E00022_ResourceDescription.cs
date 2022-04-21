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
	/// Descripción recurso
	/// </summary>

	[Index(nameof(RowidCulture), nameof(RowidResource), nameof(Description), Name = "IX_e00022_1", IsUnique = true)]
	public class E00022_ResourceDescription : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		public short RowidCulture { get; set; }

		[Required]
		public int RowidResource { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }


	}
}