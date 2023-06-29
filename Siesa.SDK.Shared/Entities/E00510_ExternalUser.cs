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
	/// Usuarios externos
	/// </summary>

	[Index(nameof(Email), Name = "IX_e00510_1", IsUnique = true)]
	public partial class E00510_ExternalUser : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(10)]
		public string Email { get; set; }

		[SDKRequired]
		[SDKStringLength(20)]
		public string Id { get; set; }

		[SDKDataEncrypt]
		[SDKSensitiveData]
		[SDKRequired]
		[SDKStringLength(255)]
		public string Password { get; set; }


	}
}