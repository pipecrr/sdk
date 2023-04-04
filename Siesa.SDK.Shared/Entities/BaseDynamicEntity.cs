using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.DataAnnotations;


namespace Siesa.SDK.Entities
{

	public abstract partial class BaseDynamicEntity<T, U> : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Record")]
		public virtual U RowidRecord { get; set; }

		[ForeignKey("EntityColumn")]
		[SDKRequired]
		public virtual int RowidEntityColumn { get; set; }

		[SDKRequired]
		public virtual short RowData { get; set; }

		[SDKStringLength(4000)]
		public virtual string? TextData { get; set; }

		[SDKRegularExpression(@"^\d{1,22}([.,]\d{0,6})?$", ErrorMessage = "El tamaño del campo {0} está fuera del rango.")]
		[Precision(28, 6)]
		public virtual decimal? NumericData { get; set; }

		public virtual DateTime? DateData { get; set; }

		public virtual int? RowidInternalEntityData { get; set; }

		public virtual int? RowidGenericEntityData { get; set; }


		public virtual T Record { get; set; }



		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00251_DynamicEntityColumn EntityColumn { get; set; }

	}
}