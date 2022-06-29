using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseUserPermission<T, U> : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Record")]
		public virtual U RowidRecord { get; set; }

		[RegularExpression(@"1|2")]
		[Required]
		public virtual byte UserType { get; set; }

		[RegularExpression(@"1|2|3")]
		[Required]
		public virtual byte AuthorizarionType { get; set; }

		[RegularExpression(@"0|1|2")]
		[Required]
		public virtual byte RestrictionType { get; set; }

		public virtual int? RowidDataVisibilityGroup { get; set; }

		public virtual int? RowidUser { get; set; }


		public virtual T Record { get; set; }



	}
}