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
	/// Cultura
	/// </summary>

	[Index(nameof(LanguageCode), nameof(CountryCode), Name = "IX_e00021_1", IsUnique = true)]
	public partial class E00021_Culture : BaseSDK<short>
	{
		[SDKIdentity]
		[Key]
		[SDKRequired]
		public override short Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(3)]
		public string LanguageCode { get; set; }

		[SDKStringLength(3)]
		public string? CountryCode { get; set; }

		[SDKRequired]
		[SDKStringLength(500)]
		public string Description { get; set; }


	}
}