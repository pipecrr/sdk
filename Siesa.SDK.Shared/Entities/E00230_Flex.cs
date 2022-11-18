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
	/// consultas flex
	/// </summary>

	public partial class E00230_Flex : BaseCompanyGroup<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		[StringLength(250)]
		public string Name { get; set; }

		[Required]
		[StringLength(500)]
		public string Description { get; set; }

		[Required]
		public string Metadata { get; set; }

		[Required]
		public string Delta { get; set; }

		public int? RowidFlexParent { get; set; }


	}
}