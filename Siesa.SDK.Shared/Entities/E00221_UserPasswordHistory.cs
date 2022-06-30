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
	/// Historial de contraseñas por usuario
	/// </summary>

	[Index(nameof(RowidUser), nameof(PasswordSequence), Name = "IX_e00221_1", IsUnique = true)]
	public partial class E00221_UserPasswordHistory : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("User")]
		[Required]
		public int RowidUser { get; set; }

		[Required]
		public short PasswordSequence { get; set; }

		[Required]
		[StringLength(128)]
		public string Password { get; set; }

		[Required]
		public DateTime PasswordAssignmentDate { get; set; }

		[Required]
		public DateTime PasswordLastUpdate { get; set; }


		[Required]
		public virtual E00220_User User { get; set; }

	}
}