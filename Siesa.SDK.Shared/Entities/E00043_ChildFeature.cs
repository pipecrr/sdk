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
	/// BL hijos
	/// </summary>

	public partial class E00043_ChildFeature : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Feature")]
		[Required]
		public int RowidFeature { get; set; }

		[ForeignKey("FeatureChild")]
		[Required]
		public int RowidFeatureChild { get; set; }


		[SDKCheckRelationship]
		public virtual E00040_Feature Feature { get; set; }

		[SDKCheckRelationship]
		public virtual E00040_Feature FeatureChild { get; set; }

	}
}