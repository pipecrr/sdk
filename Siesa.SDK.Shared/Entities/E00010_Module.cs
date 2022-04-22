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
	/// Modulos
	/// </summary>

	[Index(nameof(Id), nameof(Description), nameof(RowidResource), nameof(LicenceType), Name = "IX_e00010_1", IsUnique = true)]
	public class E00010_Module : BaseSDK<short>
	{
		[Key]
		[Required]
		public override short Rowid { get; set; }

		[Required]
		[StringLength(20)]
		public string Id { get; set; }

		[Required]
		[StringLength(250)]
		public string Description { get; set; }

		[ForeignKey("Resource")]
		[Required]
		public int RowidResource { get; set; }

		[Required]
		public byte LicenceType { get; set; }


		[Required]
		public E00020_Resource Resource { get; set; }

	}
}