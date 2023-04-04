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
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(20)]
		public override string Id { get; set; }

		[SDKRequired]
		public bool IsDefault { get; set; }

		[SDKRequired]
		public short MinimumPasswordAge { get; set; } = 0;

		[SDKRequired]
		public short MaximumPasswordAge { get; set; } = 0;

		[SDKRequired]
		public byte MinimumPasswordLength { get; set; } = 0;

		[SDKRequired]
		public bool PasswordMustComplexityRequirements { get; set; }

		[SDKRequired]
		public bool KeepPasswordHistory { get; set; }

		[SDKRequired]
		public short NumberPasswordsRemembered { get; set; } = 0;

		[SDKRequired]
		public bool AcountWillLockOut { get; set; }

		[SDKRequired]
		public byte NumberInvalidLoginAttempts { get; set; } = 0;

		[SDKRequired]
		public short AcountLockoutDuration { get; set; } = 0;


	}
}