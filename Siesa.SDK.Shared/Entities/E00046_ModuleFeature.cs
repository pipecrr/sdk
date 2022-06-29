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
	/// Característica módulo
	/// </summary>

	[Index(nameof(RowidModule), nameof(RowidFeature), Name = "IX_e00046_1", IsUnique = true)]
	public partial class E00046_ModuleFeature : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Module")]
		[Required]
		public short RowidModule { get; set; }

		[ForeignKey("Feature")]
		[Required]
		public int RowidFeature { get; set; }


		[Required]
		public virtual E00010_Module Module { get; set; }

		[Required]
		public virtual E00040_Feature Feature { get; set; }

	}
}