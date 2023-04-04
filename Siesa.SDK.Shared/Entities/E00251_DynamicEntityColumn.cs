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
	/// Columna entidad dinámica
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidDynamicEntity), nameof(Id), Name = "IX_e00251_1", IsUnique = true)]
	public partial class E00251_DynamicEntityColumn : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("DynamicEntity")]
		[SDKRequired]
		public int RowidDynamicEntity { get; set; }

		[SDKRequired]
		[SDKStringLength(100)]
		public string Id { get; set; }

		[SDKStringLength(100)]
		public string? Tag { get; set; }

		[SDKRequired]
		public short Order { get; set; }

		[SDKRequired]
		public bool IsOptional { get; set; }

		[SDKRequired]
		public bool IsDisable { get; set; }

		[SDKRequired]
		public bool IsLocked { get; set; }

		public short? Size { get; set; }

		[SDKRequired]
		public byte Decimal { get; set; }

		[SDKStringLength(4000)]
		public string? DefaultValueText { get; set; }

		[SDKRequired]
		[SDKRegularExpression(@"^\d{1,22}([.,]\d{0,6})?$", ErrorMessage = "El tamaño del campo {0} está fuera del rango.")]
		[Precision(28, 6)]
		public decimal DefaultValueNumber { get; set; }

		[SDKRequired]
		public bool ValidateMinMax { get; set; }

		[SDKRequired]
		[SDKRegularExpression(@"^\d{1,22}([.,]\d{0,6})?$", ErrorMessage = "El tamaño del campo {0} está fuera del rango.")]
		[Precision(28, 6)]
		public decimal MinValue { get; set; }

		[SDKRequired]
		[SDKRegularExpression(@"^\d{1,22}([.,]\d{0,6})?$", ErrorMessage = "El tamaño del campo {0} está fuera del rango.")]
		[Precision(28, 6)]
		public decimal MaxValue { get; set; }

		[ForeignKey("Feature")]
		public int? RowidFeature { get; set; }

		[SDKRequired]
		public enumDynamicEntityDataType DataType { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00250_DynamicEntity DynamicEntity { get; set; }

		[SDKCheckRelationship]
		public virtual E00040_Feature Feature { get; set; }

	}
}