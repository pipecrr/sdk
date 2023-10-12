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
	/// Token de usuario para API
	/// </summary>

	public partial class E00219_UserAccessToken : BaseSDK<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(20)]
		public string Id { get; set; }

		public DateOnly? ExpirationDate { get; set; }

		public DateTime? LastUsedDate { get; set; }

		[ForeignKey("User")]
		[SDKRequired]
		public int RowidUser { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[SDKRequired]
		public DateTime CreationDate { get; set; }

		[SDKSensitiveData]
		[SDKRequired]
		[SDKStringLength(40)]
		public string Token { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00220_User User { get; set; }

	}
}