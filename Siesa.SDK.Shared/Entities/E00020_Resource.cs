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
	/// Recursos
	/// </summary>

	[Index(nameof(Id), nameof(Type), Name = "IX_e00020_1", IsUnique = true)]
	public class E00020_Resource : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		[StringLength(500)]
		public string Id { get; set; }

		[Required]
		public byte Type { get; set; }


	}
}