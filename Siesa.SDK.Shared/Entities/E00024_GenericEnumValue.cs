using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;


namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Valor enum genérico
	/// </summary>

	[Index(nameof(Enum), nameof(Id), Name = "IX_e00024_1", IsUnique = true)]
	public class E00024_GenericEnumValue : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		[StringLength(100)]
		public string Enum { get; set; }

		[Required]
		[StringLength(100)]
		public string Id { get; set; }

		[Required]
		public byte Value { get; set; }


	}
}