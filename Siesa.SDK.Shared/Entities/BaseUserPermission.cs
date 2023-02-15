using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.DataAnnotations;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseUserPermission<T, U> : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Record")]
		public virtual U? RowidRecord { get; set; }

		[Required]
		public virtual PermissionUserTypes UserType { get; set; }

		[Required]
		public virtual PermissionAuthTypes AuthorizationType { get; set; }

		[Required]
		public virtual PermissionRestrictionType RestrictionType { get; set; }

		[ForeignKey("DataVisibilityGroup")]
		public virtual int? RowidDataVisibilityGroup { get; set; }

		[ForeignKey("User")]
		public virtual int? RowidUser { get; set; }


		public virtual T Record { get; set; }



		[SDKCheckRelationship]
		public virtual E00220_User User { get; set; }

		[SDKCheckRelationship]
		public virtual E00224_DataVisibilityGroup DataVisibilityGroup { get; set; }

	}
}