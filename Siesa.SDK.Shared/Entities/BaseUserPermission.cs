using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Entities.Enums;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseUserPermission<T, U> : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Record")]
		public virtual U RowidRecord { get; set; }

		[Required]
		public virtual PermissionUserTypes UserType { get; set; }

		[Required]
		public virtual PermissionAuthTypes AuthorizarionType { get; set; }

		[Required]
		public virtual PermissionRestrictionType RestrictionType { get; set; }

		[ForeignKey("DataVisibilityGroup")]
		public virtual int? RowidDataVisibilityGroup { get; set; }

		[ForeignKey("User")]
		public virtual int? RowidUser { get; set; }


		public virtual T Record { get; set; }



		public virtual E00220_User User { get; set; }

		public virtual E00224_DataVisibilityGroup DataVisibilityGroup { get; set; }

	}
}