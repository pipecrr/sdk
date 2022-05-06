using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Shared.DataAnnotations;

namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Recursos
	/// </summary>

	[Index(nameof(Id), Name = "IX_e00020_1", IsUnique = true)]
	public class E00020_Resource : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		[StringLength(500)]
		public string Id { get; set; }

		[Required]
		public byte Type { get; set; }

		public override string ToString()
		{
			return $"{Id}";
		}
	}
}