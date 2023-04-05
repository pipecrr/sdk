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
	/// Relaciona los usuarios y los roles
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidRol), nameof(RowidUser), Name = "IX_e00227_1", IsUnique = true)]
	public partial class E00227_UserRol : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Rol")]
		[SDKRequired]
		public int RowidRol { get; set; }

		[ForeignKey("User")]
		[SDKRequired]
		public int RowidUser { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00226_Rol Rol { get; set; }

		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00220_User User { get; set; }

	}
}