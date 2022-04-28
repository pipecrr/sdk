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
	/// Relaciona los usuarios y los roles
	/// </summary>

	[Index(nameof(RowidRol), nameof(RowidUser), Name = "IX_e00227_1", IsUnique = true)]
	public class E00227_UserRol : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		public int RowidRol { get; set; }

		[Required]
		public int RowidUser { get; set; }


	}
}