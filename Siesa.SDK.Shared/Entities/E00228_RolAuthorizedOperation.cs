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
	/// Operaciones autorizadas por rol
	/// </summary>

	[Index(nameof(RowidRol), nameof(RowidFeature), Name = "IX_e00228_1", IsUnique = true)]
	public class E00228_RolAuthorizedOperation : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Rol")]
		[Required]
		public int RowidRol { get; set; }

		[ForeignKey("Feature")]
		[Required]
		public int RowidFeature { get; set; }

		[Required]
		[StringLength(255)]
		public string Operation { get; set; }


		[Required]
		public virtual E00040_Feature Feature { get; set; }

		[Required]
		public virtual E00226_Rol Rol { get; set; }

	}
}