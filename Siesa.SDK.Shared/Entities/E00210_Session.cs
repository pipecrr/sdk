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
	/// Sesión
	/// </summary>

	[Index(nameof(Id), Name = "IX_e00210_1")]
	public partial class E00210_Session : BaseSDK<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		public DateTime StartDate { get; set; }

		[SDKRequired]
		[SDKStringLength(200)]
		public string Service { get; set; }

		[ForeignKey("User")]
		[SDKRequired]
		public int RowidUser { get; set; }

		[SDKStringLength(50)]
		public string? IpAddress { get; set; }

		[SDKStringLength(200)]
		public string? MachineName { get; set; }

		[SDKRequired]
		[SDKStringLength(200)]
		public string UserDataBase { get; set; }

		[SDKStringLength(2000)]
		public string? AditionalInformation { get; set; }

		[SDKStringLength(64)]
		public string? Id { get; set; }

		public string? Token { get; set; }

		public DateTime? EndDate { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00220_User User { get; set; }

	}
}