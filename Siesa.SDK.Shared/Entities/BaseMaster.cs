using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Entities.Enums;
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

		[SDKRequired]
		public virtual bool Status { get; set; }

		[SDKRequired]
		public virtual bool IsPrivate { get; set; }

		public virtual int? RowidAttachment { get; set; }

		[SDKCheckRelationship]
		public virtual E00270_Attachment Attachment { get; set; }

	}
}