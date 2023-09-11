using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace Siesa.SDK.Entities
{

	public abstract partial class BaseCompanyData<T, U> : BaseAudit<int>
	{
		[Key]
		[SDKRequired]
		public override int Rowid { get; set; }

		[ForeignKey("Record")]
		public virtual U RowidRecord { get; set; }

		[ForeignKey("Company")]
		public virtual short? RowidCompany { get; set; }

		public virtual int? RowidAttachment { get; set; }


		public virtual T Record { get; set; }



		[SDKCheckRelationship]
		public virtual E00201_Company Company { get; set; }

	}
}