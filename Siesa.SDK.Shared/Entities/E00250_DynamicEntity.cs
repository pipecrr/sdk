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
	/// Entidades dinámicas
	/// </summary>

	public partial class E00250_DynamicEntity : BaseCompanyGroup<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Feature")]
		public int? RowidFeature { get; set; }

		[ForeignKey("Company")]
		public short? RowidCompany { get; set; }

		public int? RowidGenericEntity { get; set; }

		[Required]
		[StringLength(100)]
		public string Id { get; set; }

		[StringLength(100)]
		public string? Tag { get; set; }

		[Required]
		public short Order { get; set; }

		[Required]
		public bool IsInternal { get; set; }

		[Required]
		public bool IsMultiRecord { get; set; }

		[Required]
		public bool IsOptional { get; set; }

		[Required]
		public bool IsDisable { get; set; }

		[Required]
		public bool IsLocked { get; set; }


		[SDKCheckRelationship]
		public virtual E00201_Company Company { get; set; }

		[SDKCheckRelationship]
		public virtual E00040_Feature Feature { get; set; }

	}
}