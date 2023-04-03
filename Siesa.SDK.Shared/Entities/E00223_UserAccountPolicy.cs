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
	/// Politicas de la cuenta de usuario
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(Id), Name = "IX_e00223_1", IsUnique = true)]
	public partial class E00223_UserAccountPolicy : BaseMaster<int, string>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		[StringLength(20)]
		public override string Id { get; set; }

		[Required]
		public bool IsDefault { get; set; }

		[Required]
		public short MinimumPasswordAge { get; set; } = 0;

		[Required]
		public short MaximumPasswordAge { get; set; } = 0;

		[Required]
		public byte MinimumPasswordLength { get; set; } = 0;

		[Required]
		public bool PasswordMustComplexityRequirements { get; set; }

		[Required]
		public bool KeepPasswordHistory { get; set; }

		[Required]
		public short NumberPasswordsRemembered { get; set; } = 0;

		[Required]
		public bool AcountWillLockOut { get; set; }

		[Required]
		public byte NumberInvalidLoginAttempts { get; set; } = 0;

		[Required]
		public short AcountLockoutDuration { get; set; } = 0;


	}
}