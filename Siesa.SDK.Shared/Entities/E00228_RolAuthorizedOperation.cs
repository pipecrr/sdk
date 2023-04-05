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
	/// Operaciones autorizadas por rol
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidRol), nameof(RowidFeature), Name = "IX_e00228_1", IsUnique = true)]
	public partial class E00228_RolAuthorizedOperation : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Rol")]
		[SDKRequired]
		public int RowidRol { get; set; }

		[ForeignKey("Feature")]
		[SDKRequired]
		public int RowidFeature { get; set; }

		[SDKRequired]
		[SDKStringLength(255)]
		public string Operation { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00040_Feature Feature { get; set; }

		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00226_Rol Rol { get; set; }

	}
}