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
	/// Consultas flex que nacen desde la fábrica
	/// </summary>

	[Index(nameof(Id), Name = "IX_e00232_1", IsUnique = true)]
	public partial class E00232_FlexProduct : BaseSDK<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("ResourceName")]
		[SDKRequired]
		public int RowidResourceName { get; set; }

		[ForeignKey("ResourceDescription")]
		public int? RowidResourceDescription { get; set; }

		public string? Metadata { get; set; }

		[SDKRequired]
		[SDKStringLength(50)]
		public string Id { get; set; } = string.Empty;


		[SDKCheckRelationship]
		public virtual E00020_Resource? ResourceName { get; set; }

		[SDKCheckRelationship]
		public virtual E00020_Resource? ResourceDescription { get; set; }

	}
}