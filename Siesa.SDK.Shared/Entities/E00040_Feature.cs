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
	/// Características
	/// </summary>

	[Index(nameof(RowidResource), Name = "IX_e00040_1", IsUnique = true)]
	[Index(nameof(BusinessName), Name = "IX_e00040_2")]
	public partial class E00040_Feature : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Resource")]
		[Required]
		public int RowidResource { get; set; }

		[Required]
		[StringLength(100)]
		public string BusinessName { get; set; }


		[SDKCheckRelationship]
		[Required]
		public virtual E00020_Resource Resource { get; set; }

	}
}