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
	/// Permisos sobre los registros de la tabla SuiteMenuCustom
	/// </summary>

	public class U00067_SuiteMenuCustom : BaseAudit<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		public byte UserType { get; set; }

		[Required]
		public byte AuthorizarionType { get; set; }

		[Required]
		public byte RestrictionType { get; set; }

		public int? RowidTeam { get; set; }

		public int? RowidUser { get; set; }

		public int? RowidRecord { get; set; }


	}
}