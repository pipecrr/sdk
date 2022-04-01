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
	/// Tabla para personalizar el texto de los recursos
	/// </summary>

	[Index(nameof(RowidResourceDescription), nameof(Description), Name = "IX_e00023_1", IsUnique = true)]
	public class E00023_ResourceCustomDescription : BaseEntity<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("ResourceDescription")]
		[Required]
		public  int RowidResourceDescription { get; set; }

		[Required]
		[StringLength(2000)]
		public  string Description { get; set; }


		public E00020_Resource ResourceDescription { get; set; }

	}
}