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
	/// Usuarios
	/// </summary>

	[Index(nameof(Id), nameof(Description), Name = "IX_e00220_1", IsUnique = true)]
	public class E00220_User : BaseEntity<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		[StringLength(500)]
		public string Id { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }

		public int? RowidUserReportTo { get; set; }


	}
}