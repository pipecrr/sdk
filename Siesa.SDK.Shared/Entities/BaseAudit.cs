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
		[SDKRequired]
		public virtual DateTime CreationDate { get; set; } = DateTime.UtcNow;

		public virtual DateTime? LastUpdateDate { get; set; } = DateTime.UtcNow;

		[SDKStringLength(2000)]
		public virtual string? Source { get; set; }

		[SDKRequired]
		[ForeignKey("UserCreates")]
		public virtual int RowidUserCreates { get; set; }
		
		[ForeignKey("UserLastUpdate")]
		public virtual int? RowidUserLastUpdate { get; set; }

		public virtual int? RowidSession { get; set; }

		public virtual E00220_User UserCreates { get; set; }
		public virtual E00220_User? UserLastUpdate { get; set; }


	}
}