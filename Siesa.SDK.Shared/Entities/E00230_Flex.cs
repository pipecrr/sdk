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
	/// consultas flex
	/// </summary>

	public partial class E00230_Flex : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(250)]
		public string Name { get; set; } = string.Empty;

		[SDKStringLength(500)]
		public string? Description { get; set; }

		public string? Metadata { get; set; }


	}
}