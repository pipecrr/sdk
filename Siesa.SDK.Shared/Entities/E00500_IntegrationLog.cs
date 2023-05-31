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
	/// Registro de integraciones 
	/// </summary>

	[Index(nameof(Guid), Name = "IX_e00500_1", IsUnique = true)]
	public partial class E00500_IntegrationLog : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(36)]
		public string Guid { get; set; }

		[SDKRequired]
		public enumSDKIntegrationStatus Status { get; set; } = enumSDKIntegrationStatus.Queued;

		public string? Result { get; set; }


	}
}