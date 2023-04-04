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
	/// Valor enum genérico
	/// </summary>

	[Index(nameof(Id), Name = "IX_e00024_1", IsUnique = true)]
	public partial class E00024_Enum : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(100)]
		public string Id { get; set; }

		[SDKRequired]
		[SDKStringLength(100)]
		public string Nombre { get; set; }


	}
}