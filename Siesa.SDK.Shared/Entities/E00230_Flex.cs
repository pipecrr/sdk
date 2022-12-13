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
	/// consultas flex
	/// </summary>

	public partial class E00230_Flex : BaseCompanyGroup<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[SDKRequired]
		[StringLength(250)]
		public string Name { get; set; }

		[StringLength(500)]
		public string? Description { get; set; }

		public string? Metadata { get; set; }

		public string? Delta { get; set; }

		[ForeignKey("FlexParent")]
		public int? RowidFlexParent { get; set; }

		[ForeignKey("Feature")]
		public int? RowidFeature { get; set; }


		[SDKCheckRelationship]
		public virtual E00230_Flex FlexParent { get; set; }

		public virtual E00040_Feature Feature { get; set; }

	}
}