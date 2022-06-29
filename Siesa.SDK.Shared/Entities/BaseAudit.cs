using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseAudit<T> : BaseSDK<T>
	{
		

		[Required]
		public virtual DateTime CreationDate { get; set; }

		public virtual DateTime? LastUpdateDate { get; set; }

		[StringLength(2000)]
		public virtual string? Source { get; set; }

		[ForeignKey("UserCreates")]
		[Required]
		public virtual int RowidUserCreates { get; set; }

		[ForeignKey("UserLastUpdate")]
		public virtual int? RowidUserLastUpdate { get; set; }

		public virtual int? RowidSession { get; set; }


		[Required]
		public virtual E00220_User UserCreates { get; set; }

		public virtual E00220_User UserLastUpdate { get; set; }

	}
}