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
	/// Modulos
	/// </summary>

	[Index(nameof(Id), Name = "IX_e00010_1", IsUnique = true)]
	public partial class E00010_Service : BaseSDK<short>
	{
		[SDKIdentity]
		[Key]
		[SDKRequired]
		public override short Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(20)]
		public string Id { get; set; }

		[SDKRequired]
		[SDKStringLength(250)]
		public string Description { get; set; }

		[ForeignKey("Resource")]
		[SDKRequired]
		public int RowidResource { get; set; }

		[SDKRequired]
		public byte LicenceType { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00020_Resource Resource { get; set; }

	}
}