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
	/// Grupos de trabajo
	/// </summary>
	[SDKLogEntity]
	[Index(nameof(RowidCompanyGroup), nameof(Id), Name = "IX_e00224_1", IsUnique = true)]
	public partial class E00224_DataVisibilityGroup : BaseMasterCompanyGroup<string>
	{
		[Required]
		[StringLength(20)]
		public override string Id { get; set; }

		[ForeignKey("Company")]
		public short? RowidCompany { get; set; }


		[SDKCheckRelationship]
		public virtual E00201_Company Company { get; set; }

	}
}