using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.Global.Enums;



namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Usuario
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(Id), Name = "IX_e00220_1", IsUnique = true)]
	public partial class E00220_User : BaseMaster<int, string>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(20)]
		public override string Id { get; set; }

		[SDKMaxLength(128)]
		public byte[]? Sid { get; set; }

		[SDKRequired]
		[SDKStringLength(1024)]
		public string Path { get; set; }

		[SDKDataEncrypt]
		[SDKStringLength(128)]
		public string? Password { get; set; }

		[ForeignKey("Culture")]
		[SDKRequired]
		public short RowidCulture { get; set; }

		[SDKRequired]
		public DateTime PasswordAssignmentDate { get; set; }

		[SDKRequired]
		public DateTime PasswordLastUpdate { get; set; }

		// [ForeignKey("UserAccountPolicy")]
		// public int? RowidUserAccountPolicy { get; set; }

		[SDKRequired]
		public bool ChangePasswordFirstLogin { get; set; }

		[SDKRequired]
		public DateTime StartDateValidity { get; set; }

		public DateTime? EndDateValidity { get; set; }

		public DateTime? LastDateValidLogin { get; set; }

		public DateTime? LastDateInvalidLogin { get; set; }

		[SDKRequired]
		public byte NumberIncorrectPasswords { get; set; } = 0;

		[SDKRequired]
		public bool IsLocked { get; set; }

		public DateTime? LastDateLocked { get; set; }

		[SDKStringLength(256)]
		public string? PasswordRecoveryEmail { get; set; }

		[ForeignKey("UserSubstitute")]
		public int? RowidUserSubstitute { get; set; }

		public DateTime? SubstitutionStartDate { get; set; }

		public DateTime? SubstitutionEndDate { get; set; }

		[SDKRequired]
		public bool AllowAccessSustitutedUser { get; set; }

		[SDKRequired]
		public bool HasScheduledAccess { get; set; }

		[SDKRequired]
		public bool HasTotalAccessMonday { get; set; }

		[SDKRequired]
		public bool HasTotalAccessTuesday { get; set; }

		[SDKRequired]
		public bool HasTotalAccessWednesday { get; set; }

		[SDKRequired]
		public bool HasTotalAccessThursday { get; set; }

		[SDKRequired]
		public bool HasTotalAccessFriday { get; set; }

		[SDKRequired]
		public bool HasTotalAccessSaturday { get; set; }

		[SDKRequired]
		public bool HasTotalAccessSunday { get; set; }

		[ForeignKey("UserReportTo")]
		public int? RowidUserReportTo { get; set; }

		[SDKRequired]
		public bool IsAdministrator { get; set; }

		[SDKRequired]
		public bool IsVisibilityAdministrator { get; set; }

		// [ForeignKey("AttachmentUserProfilePicture")]
		// public int? RowidAttachmentUserProfilePicture { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00021_Culture Culture { get; set; }

		[SDKCheckRelationship]
		public virtual E00220_User UserSubstitute { get; set; }

		[SDKCheckRelationship]
		public virtual E00220_User UserReportTo { get; set; }

		// [SDKCheckRelationship]
		// public virtual E00223_UserAccountPolicy UserAccountPolicy { get; set; }

		//public virtual E00271_AttachmentDetail AttachmentUserProfilePicture { get; set; }

	}
}