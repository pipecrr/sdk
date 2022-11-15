using Newtonsoft.Json;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Shared.Json;
using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Entities.Enums;


namespace Siesa.SDK.Entities
{
	/// <summary>
	/// Entidades dinámicas
	/// </summary>

	public partial class E00250_Entity : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		[StringLength(60)]
		public string Entity { get; set; }

		[Required]
		public int RowidGenericEntity { get; set; }

		[Required]
		public int RowidDocument { get; set; }

		[Required]
		public byte FilterType { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }


	}
}