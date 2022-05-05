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
	/// Sobrescritura de operaciones  autorizadas por rol  y usuario
	/// </summary>

	[Index(nameof(RowidRolAuthorizedOperation), nameof(RowidCompany), nameof(RowidUser), Name = "IX_e00229_1", IsUnique = true)]
	public class E00229_OverwriteRolAuthorizedOperation : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("RolAuthorizedOperation")]
		[Required]
		public int RowidRolAuthorizedOperation { get; set; }

		[ForeignKey("Company")]
		public short? RowidCompany { get; set; }

		[ForeignKey("User")]
		[Required]
		public int RowidUser { get; set; }

		[Required]
		[StringLength(225)]
		public string Operation { get; set; }


		public E00201_Company Company { get; set; }

		[Required]
		public E00228_RolAuthorizedOperation RolAuthorizedOperation { get; set; }

		[Required]
		public E00220_User User { get; set; }

	}
}