using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Entities.Enums;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseMaster<T, U> : BaseAudit<T>
	{
		

		public virtual U Id { get; set; }

		[Required]
		[StringLength(250)]
		public virtual string Name { get; set; }

		[Required]
		[StringLength(2000)]
		public virtual string Description { get; set; }

		[Required]
		public virtual int Status { get; set; }

		[Required]
		public virtual bool IsPrivate { get; set; }

		[ForeignKey("Attachment")]
		public virtual int? RowidAttachment { get; set; }


		[SDKCheckRelationship]
		public virtual E00270_Attachment Attachment { get; set; }

	}
}