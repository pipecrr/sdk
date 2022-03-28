using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;


namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Cadenas de texto que se le muestran al usuario dependiendo de la cultura.
	/// </summary>

	[Index(nameof(RowidResource), nameof(RowidCultura), Name = "IX_e00022_1", IsUnique = true)]
	public class E00022_ResourceDescription
	{
		[Key]
		[Required]
		public  int Rowid { get; set; }

		[ForeignKey("Resource")]
		[Required]
		public  int RowidResource { get; set; }

		[ForeignKey("Cultura")]
		[Required]
		public  int RowidCultura { get; set; }

		[Required]
		[StringLength(2000)]
		public  string Description { get; set; }


		public E00020_Resource Resource { get; set; }

		public E00021_Culture Cultura { get; set; }

	}
}