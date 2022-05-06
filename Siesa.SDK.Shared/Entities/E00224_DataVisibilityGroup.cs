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
	/// Grupos de trabajo
	/// </summary>
	[SDKLogEntity]
	public class E00224_DataVisibilityGroup : BaseMasterCompanyGroup<string>
	{
		[Required]
		[StringLength(20)]
		public override string Id { get; set; }

		[ForeignKey("Company")]
		public short? RowidCompany { get; set; }


		public E00201_Company Company { get; set; }

	}
}