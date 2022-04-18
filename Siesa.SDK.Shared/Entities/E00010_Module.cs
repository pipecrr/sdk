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
	/// Módulos
	/// </summary>

	[Index(nameof(Id), nameof(LicenseType), nameof(RowidResource), Name = "IX_e00010_1", IsUnique = true)]
	public class E00010_Module
	{
		[Key]
		[Required]
		public int Rowid { get; set; }

		[RegularExpression(@"1|2|3")]
		[Required]
		public byte Id { get; set; }

		[Required]
		[StringLength(250)]
		public string Description { get; set; }

		[ForeignKey("Resource")]
		[Required]
		public int RowidResource { get; set; }

		[Required]
		public byte LicenseType { get; set; } = 1;


		[Required]
		public E00020_Resource Resource { get; set; }

	}
}