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
	/// BL hijos
	/// </summary>

	[Index(nameof(RowidFeature), Name = "IX_e00043_1")]
	public partial class E00043_ChildFeature : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Feature")]
		[SDKRequired]
		public int RowidFeature { get; set; }

		[ForeignKey("FeatureChild")]
		[SDKRequired]
		public int RowidFeatureChild { get; set; }


		[SDKCheckRelationship]
		public virtual E00040_Feature Feature { get; set; }

		[SDKCheckRelationship]
		public virtual E00040_Feature FeatureChild { get; set; }

	}
}