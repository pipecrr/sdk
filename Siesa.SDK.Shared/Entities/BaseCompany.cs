using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace Siesa.SDK.Entities
{

	public abstract partial class BaseCompany<T> : BaseAudit<T>
	{
		

		[ForeignKey("Company")]
		[SDKRequired]
		public virtual short RowidCompany { get; set; }


		[SDKCheckRelationship]
		[SDKRequired]
		public virtual E00201_Company Company { get; set; }

	}
}