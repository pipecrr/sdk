using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Entities.Enums;


namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Relaciona los usuarios y los roles
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidRol), nameof(RowidUser), Name = "IX_e00227_1", IsUnique = true)]
	public partial class E00227_UserRol : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Rol")]
		[Required]
		public int RowidRol { get; set; }

		[ForeignKey("User")]
		[Required]
		public int RowidUser { get; set; }


		[Required]
		public virtual E00226_Rol Rol { get; set; }

		[Required]
		public virtual E00220_User User { get; set; }

	}
}