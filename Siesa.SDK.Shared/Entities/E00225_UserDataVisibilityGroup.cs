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
	/// Grupos
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidDataVisibilityGroup), nameof(RowidUser), Name = "IX_e00225_1", IsUnique = true)]
	public partial class E00225_UserDataVisibilityGroup : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("DataVisibilityGroup")]
		[SDKRequired]
		public int RowidDataVisibilityGroup { get; set; }

		[SDKRequired]
		public int RowidUser { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00224_DataVisibilityGroup DataVisibilityGroup { get; set; }

	}
}