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
	/// Grupo compañía
	/// </summary>

	[Index(nameof(Id), Name = "IX_e00200_1", IsUnique = true)]
	public partial class E00200_CompanyGroup : BaseSDK<short>
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
		public string Name { get; set; }

		[ForeignKey("Logo")]
		public int? RowidLogo { get; set; }


		public virtual E00271_AttachmentDetail Logo { get; set; }

	}
}