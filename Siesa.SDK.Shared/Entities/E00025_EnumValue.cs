using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Entities.Enums;


namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Valores de los enums
	/// </summary>

	[Index(nameof(RowidEnum), nameof(Id), nameof(Value), Name = "IX_e00025_1", IsUnique = true)]
	public partial class E00025_EnumValue : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		[StringLength(100)]
		public string Id { get; set; }

		[Required]
		public byte Value { get; set; }

		[ForeignKey("Enum")]
		[Required]
		public int RowidEnum { get; set; }


		[SDKCheckRelationship]
		[Required]
		public virtual E00024_Enum Enum { get; set; }

	}
}