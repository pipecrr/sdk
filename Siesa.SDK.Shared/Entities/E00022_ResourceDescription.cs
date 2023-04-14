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
	/// Descripción recurso
	/// </summary>

	[Index(nameof(RowidCulture), nameof(RowidResource), Name = "IX_e00022_1", IsUnique = true)]
	public partial class E00022_ResourceDescription : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Culture")]
		[SDKRequired]
		public short RowidCulture { get; set; }

		[ForeignKey("Resource")]
		[SDKRequired]
		public int RowidResource { get; set; }

		[SDKRequired]
		[SDKStringLength(500)]
		public string Description { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00020_Resource Resource { get; set; }

		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00021_Culture Culture { get; set; }

	}
}