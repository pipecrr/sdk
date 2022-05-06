using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;


namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Usuario
	/// </summary>
	[SDKLogEntity]
	public class E00220_User : BaseMaster<string>
	{
		[Required]
		[StringLength(20)]
		public override string Id { get; set; }

		[MaxLength(128)]
		public byte[]? Sid { get; set; }

		[Required]
		[StringLength(1024)]
		public string Path { get; set; }

		[StringLength(128)]
		public string? Password { get; set; }

		[ForeignKey("Culture")]
		[Required]
		public short RowidCulture { get; set; }

		[Required]
		public DateTime PasswordAssignmentDate { get; set; }

		[Required]
		public DateTime PasswordLastUpdate { get; set; }

		// [ForeignKey("UserAccountPolicy")]
		// public int? RowidUserAccountPolicy { get; set; }

		[Required]
		public bool ChangePasswordFirstLogin { get; set; }

		[Required]
		public DateTime StartDateValidity { get; set; }

		public DateTime? EndDateValidity { get; set; }

		public DateTime? LastDateValidLogin { get; set; }

		public DateTime? LastDateInvalidLogin { get; set; }

		[Required]
		public byte NumberIncorrectPasswords { get; set; } = 0;

		[Required]
		public bool IsLocked { get; set; }

		public DateTime? LastDateLocked { get; set; }

		[StringLength(256)]
		public string? PasswordRecoveryEmail { get; set; }

		[ForeignKey("UserSubstitute")]
		public int? RowidUserSubstitute { get; set; }

		public DateTime? SubstitutionStartDate { get; set; }

		public DateTime? SubstitutionEndDate { get; set; }

		[Required]
		public bool AllowAccessSustitutedUser { get; set; }

		[Required]
		public bool HasScheduledAccess { get; set; }

		[Required]
		public bool HasTotalAccessMonday { get; set; }

		[Required]
		public bool HasTotalAccessTuesday { get; set; }

		[Required]
		public bool HasTotalAccessWednesday { get; set; }

		[Required]
		public bool HasTotalAccessThursday { get; set; }

		[Required]
		public bool HasTotalAccessFriday { get; set; }

		[Required]
		public bool HasTotalAccessSaturday { get; set; }

		[Required]
		public bool HasTotalAccessSunday { get; set; }

		[ForeignKey("UserReportTo")]
		public int? RowidUserReportTo { get; set; }


		[Required]
		public E00021_Culture Culture { get; set; }

		public E00220_User UserSubstitute { get; set; }

		public E00220_User UserReportTo { get; set; }

		// public E00223_UserAccountPolicy UserAccountPolicy { get; set; }

	}
}