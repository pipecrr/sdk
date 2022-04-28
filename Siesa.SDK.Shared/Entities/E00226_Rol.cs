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
	/// Tabla de roles
	/// </summary>

	public class E00226_Rol : BaseMasterCompanyGroup<int, int, string>
	{
		[Required]
		[StringLength(20)]
		public override string Id { get; set; }

		public int? RowidCompany { get; set; }


	}
}