using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.DataAnnotations;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseMaster<T, U> : BaseAudit<T>
	{
		

		public virtual U Id { get; set; }

		[SDKRequired]
		[StringLength(250)]
		public virtual string Name { get; set; }

		[SDKRequired]
		[StringLength(2000)]
		public virtual string Description { get; set; }

		[Required]
		public virtual enumStatusBaseMaster Status { get; set; }

		[Required]
		public virtual bool IsPrivate { get; set; }

		[ForeignKey("Attachment")]
		public virtual int? RowidAttachment { get; set; }


	}
}