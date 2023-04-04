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
	/// Historial de contraseñas por usuario
	/// </summary>

	[Index(nameof(RowidUser), nameof(PasswordSequence), Name = "IX_e00221_1", IsUnique = true)]
	public partial class E00221_UserPasswordHistory : BaseSDK<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("User")]
		[SDKRequired]
		public int RowidUser { get; set; }

		[SDKRequired]
		public short PasswordSequence { get; set; }

		[SDKRequired]
		[SDKStringLength(128)]
		public string Password { get; set; }

		[SDKRequired]
		public DateTime PasswordAssignmentDate { get; set; }

		[SDKRequired]
		public DateTime PasswordLastUpdate { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00220_User User { get; set; }

	}
}