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
	/// Entidades dinámicas
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidCompanyGroup), nameof(RowidFeature), nameof(RowidCompany), nameof(RowidGenericEntity), nameof(Id), Name = "IX_e00250_1", IsUnique = true)]
	public partial class E00250_DynamicEntity : BaseCompanyGroup<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Feature")]
		public int? RowidFeature { get; set; }

		[ForeignKey("Company")]
		public short? RowidCompany { get; set; }

		public int? RowidGenericEntity { get; set; }

		[SDKRequired]
		[SDKStringLength(100)]
		public string Id { get; set; }

		[SDKStringLength(100)]
		public string? Tag { get; set; }

		[SDKRequired]
		public short Order { get; set; }

		[SDKRequired]
		public bool IsInternal { get; set; }

		[SDKRequired]
		public bool IsMultiRecord { get; set; }

		[SDKRequired]
		public bool IsOptional { get; set; }

		[SDKRequired]
		public bool IsDisable { get; set; }

		[SDKRequired]
		public bool IsLocked { get; set; }


		[SDKCheckRelationship]
		public virtual E00201_Company Company { get; set; }

		[SDKCheckRelationship]
		public virtual E00040_Feature Feature { get; set; }

	}
}