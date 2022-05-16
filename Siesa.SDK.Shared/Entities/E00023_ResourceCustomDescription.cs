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
	/// Descripción recurso personalizado
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidResourceDescription), Name = "IX_e00023_1", IsUnique = true)]
	public class E00023_ResourceCustomDescription : BaseAudit<int>
	{
		[SDKIdentity]
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("ResourceDescription")]
		[Required]
		public int RowidResourceDescription { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }


		[Required]
		public virtual E00022_ResourceDescription ResourceDescription { get; set; }

	}
}