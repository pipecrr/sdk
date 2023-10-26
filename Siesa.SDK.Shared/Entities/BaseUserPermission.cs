using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace Siesa.SDK.Entities
{

	public abstract partial class BaseUserPermission<T, U> : BaseAudit<int> where U : struct
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Record")]
		public virtual U? RowidRecord { get; set; }

		[SDKRequired]
		public virtual EnumPermissionUserTypes UserType { get; set; }

		[SDKRequired]
		public virtual EnumPermissionAuthTypes AuthorizationType { get; set; }

		[SDKRequired]
		public virtual EnumPermissionRestrictionType RestrictionType { get; set; }

		[ForeignKey("DataVisibilityGroup")]
		public virtual int? RowidDataVisibilityGroup { get; set; }

		[ForeignKey("User")]
		public virtual int? RowidUser { get; set; }


		public virtual T? Record { get; set; }



		[SDKCheckRelationship]
		public virtual E00220_User User { get; set; }

		[SDKCheckRelationship]
		public virtual E00224_DataVisibilityGroup DataVisibilityGroup { get; set; }

	}
}