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
	/// Suite Menu
	/// </summary>

	[Index(nameof(RowidSuite), nameof(RowidMenu), nameof(Order), Name = "IX_e00062_1", IsUnique = true)]
	public class E00062_SuiteMenu : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		public short RowidSuite { get; set; }

		[Required]
		public int RowidMenu { get; set; }

		[Required]
		public byte Order { get; set; }


	}
}