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
	public class E00229_OverwirteRolAuthorizedOperation : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		public int RowidRolAuthorizedOperation { get; set; }

		public int? RowidCompany { get; set; }

		[Required]
		public int RowidUser { get; set; }

		[Required]
		[StringLength(225)]
		public string Operation { get; set; }


	}
}