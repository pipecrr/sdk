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
	/// Modificaciones sobre registros flex, aplica tambien para listview
	/// </summary>

	public partial class E00231_FlexDelta : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(250)]
		public string Name { get; set; } = string.Empty;

		public string? Delta { get; set; }

		[ForeignKey("FlexParent")]
		public int? RowidFlexParent { get; set; }

		[ForeignKey("Feature")]
		public int? RowidFeature { get; set; }

		[ForeignKey("FlexProduct")]
		public int? RowidFlexProduct { get; set; }


		[SDKCheckRelationship]
		public virtual E00230_Flex? FlexParent { get; set; }

		[SDKCheckRelationship]
		public virtual E00040_Feature? Feature { get; set; }

		[SDKCheckRelationship]
		public virtual E00232_FlexProduct? FlexProduct { get; set; }

	}
}