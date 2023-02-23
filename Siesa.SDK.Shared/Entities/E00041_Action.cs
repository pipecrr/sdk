using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.Global.Enums;



namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Acción
	/// </summary>

	[Index(nameof(RowidResource), Name = "IX_e00041_1", IsUnique = true)]
	public partial class E00041_Action : BaseSDK<int>
	{
		[SDKIdentity]
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Resource")]
		[Required]
		public int RowidResource { get; set; }

		public short? Priority { get; set; }

		[StringLength(7)]
		public string? HexColor { get; set; }

		[StringLength(250)]
		public string? IconClass { get; set; }

		[StringLength(250)]
		public string? Style { get; set; }


		[SDKCheckRelationship]
		[Required]
		public virtual E00020_Resource Resource { get; set; }

	}
}