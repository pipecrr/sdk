using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.Global.Enums;



namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Tabla para token de recuperación de contraseña
	/// </summary>

	[Index(nameof(Id), Name = "IX_e00218_1", IsUnique = true)]
	public partial class E00218_UserRecoveryToken : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(40)]
		public string Id { get; set; }

		[SDKRequired]
		public bool Used { get; set; }

		[ForeignKey("User")]
		[SDKRequired]
		public int RowidUser { get; set; }


		public virtual E00220_User User { get; set; }

	}
}