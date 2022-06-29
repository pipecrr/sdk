using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseAdditionalData<T, U> : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Record")]
		public virtual U RowidRecord { get; set; }

		public virtual short? RowidCompany { get; set; }

		public virtual int? RowidAttachment { get; set; }


		public virtual T Record { get; set; }



	}
}