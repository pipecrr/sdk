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
	/// Compañía
	/// </summary>

	public partial class E00201_Company : BaseSDK<short>
	{
		[SDKIdentity]
		[Key]
		[Required]
		public override short Rowid { get; set; }

		[ForeignKey("CompanyGroup")]
		[Required]
		public short RowidCompanyGroup { get; set; }

		[Required]
		[StringLength(20)]
		public string Id { get; set; }

		[Required]
		[StringLength(250)]
		public string Name { get; set; }


		[SDKCheckRelationship]
		[Required]
		public virtual E00200_CompanyGroup CompanyGroup { get; set; }

	}
}