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
	/// Acción
	/// </summary>

	[Index(nameof(RowidResource), Name = "IX_e00041_1", IsUnique = true)]
	public class E00041_Action : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Resource")]
		[Required]
		public int RowidResource { get; set; }


		[Required]
		public E00020_Resource Resource { get; set; }

	}
}