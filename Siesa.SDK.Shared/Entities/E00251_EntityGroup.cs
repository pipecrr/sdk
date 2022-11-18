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
	/// Grupo de entidades dinámicas
	/// </summary>

	public partial class E00251_EntityGroup : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[Required]
		[StringLength(100)]
		public string Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Tag { get; set; }

		[Required]
		public bool IsInternal { get; set; }

		[Required]
		public bool IsMultiline { get; set; }

		[Required]
		public bool IsOptional { get; set; }

		[Required]
		public bool IsDisabled { get; set; }

		[Required]
		public bool IsLocked { get; set; }

		[ForeignKey("CompanyGroup")]
		public short? RowidCompanyGroup { get; set; }

		[ForeignKey("Company")]
		public short? RowidCompany { get; set; }

		[ForeignKey("Entity")]
		[Required]
		public int RowidEntity { get; set; }


		[SDKCheckRelationship]
		public virtual E00200_CompanyGroup CompanyGroup { get; set; }

		[SDKCheckRelationship]
		[Required]
		public virtual E00201_Company Company { get; set; }

		[SDKCheckRelationship]
		[Required]
		public virtual E00250_Entity Entity { get; set; }

	}
}