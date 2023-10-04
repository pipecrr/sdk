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
	/// Tabla de roles
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidCompanyGroup), nameof(Id), Name = "IX_e00226_1", IsUnique = true)]
	public partial class E00226_Rol : BaseMasterCompanyGroup<string>
	{
		[SDKRequired]
		[SDKStringLength(20)]
		public override string Id { get; set; }

		[ForeignKey("Company")]
		public short? RowidCompany { get; set; }

		public string? CalculatedPermissions { get; set; }


		[SDKCheckRelationship]
		public virtual E00201_Company Company { get; set; }

	}
}