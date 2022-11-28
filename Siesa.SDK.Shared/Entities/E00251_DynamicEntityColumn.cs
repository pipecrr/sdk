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
	/// Columna entidad dinámica
	/// </summary>

	public partial class E00251_DynamicEntityColumn : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("DynamicEntity")]
		[Required]
		public int RowidDynamicEntity { get; set; }

		[Required]
		[StringLength(100)]
		public string Id { get; set; }

		[StringLength(100)]
		public string? Tag { get; set; }

		[Required]
		public short Order { get; set; }

		[Required]
		public bool IsOptional { get; set; }

		[Required]
		public bool IsDisable { get; set; }

		[Required]
		public bool IsLocked { get; set; }

		public short? Size { get; set; }

		[Required]
		public byte Decimal { get; set; }

		[StringLength(4000)]
		public string? DefaultValueText { get; set; }

		[Required]
		[RegularExpression(@"^\d{1,22}([.,]\d{0,6})?$", ErrorMessage = "El tamaño del campo {0} está fuera del rango.")]
		public decimal DefaultValueNumber { get; set; }

		[Required]
		public bool ValidateMinMax { get; set; }

		[Required]
		[RegularExpression(@"^\d{1,22}([.,]\d{0,6})?$", ErrorMessage = "El tamaño del campo {0} está fuera del rango.")]
		public decimal MinValue { get; set; }

		[Required]
		[RegularExpression(@"^\d{1,22}([.,]\d{0,6})?$", ErrorMessage = "El tamaño del campo {0} está fuera del rango.")]
		public decimal MaxValue { get; set; }

		[ForeignKey("Feature")]
		public int? RowidFeature { get; set; }


		[SDKCheckRelationship]
		[Required]
		public virtual E00250_DynamicEntity DynamicEntity { get; set; }

		public virtual E00040_Feature Feature { get; set; }

	}
}