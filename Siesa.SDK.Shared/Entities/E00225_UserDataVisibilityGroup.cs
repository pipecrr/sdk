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
	/// Grupos
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidDataVisibilityGroup), nameof(RowidUser), Name = "IX_e00225_1", IsUnique = true)]
	public partial class E00225_UserDataVisibilityGroup : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("DataVisibilityGroup")]
		[Required]
		public int RowidDataVisibilityGroup { get; set; }

		[Required]
		public int RowidUser { get; set; }


		[Required]
		public virtual E00224_DataVisibilityGroup DataVisibilityGroup { get; set; }

	}
}