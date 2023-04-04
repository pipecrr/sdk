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
	/// Valores de los enums
	/// </summary>

	[Index(nameof(RowidEnum), nameof(Id), nameof(Value), Name = "IX_e00025_1", IsUnique = true)]
	public partial class E00025_EnumValue : BaseSDK<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[SDKRequired]
		[SDKStringLength(100)]
		public string Id { get; set; }

		[SDKRequired]
		public byte Value { get; set; }

		[ForeignKey("Enum")]
		[SDKRequired]
		public int RowidEnum { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00024_Enum Enum { get; set; }

	}
}