using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.DataAnnotations;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseAudit<T> : BaseSDK<T>
	{
		

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
		public virtual DateTime CreationDate { get; set; } = DateTime.UtcNow;

		public virtual DateTime? LastUpdateDate { get; set; } = DateTime.UtcNow;

		[StringLength(2000)]
		public virtual string? Source { get; set; }

		[Required]
		public virtual int RowidUserCreates { get; set; }

		public virtual int? RowidUserLastUpdate { get; set; }

		public virtual int? RowidSession { get; set; }


	}
}