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
	/// consultas flex
	/// </summary>

	public partial class E00230_Flex : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(2)]
		[SDKRegularExpression(@"^\d{1,7}([.,]\d{0,2})?$")]
		public string Name { get; set; }
		
		[SDKStringLength(5)]
		public string? Description { get; set; }

		public string? Metadata { get; set; }

		public string? Delta { get; set; }

		[ForeignKey("FlexParent")]
		public int? RowidFlexParent { get; set; }

		[ForeignKey("Feature")]
		public int? RowidFeature { get; set; }


		[SDKCheckRelationship]
		public virtual E00230_Flex FlexParent { get; set; }

		[SDKCheckRelationship]
		public virtual E00040_Feature Feature { get; set; }

	}
}