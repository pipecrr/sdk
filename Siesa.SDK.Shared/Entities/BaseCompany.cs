using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Entities.Enums;
using Siesa.SDK.Shared.DataAnnotations;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseCompany<T> : BaseAudit<T>
	{
		

		[ForeignKey("Company")]
		[Required]
		public virtual short RowidCompany { get; set; }


		[SDKCheckRelationship]
		[Required]
		public virtual E00201_Company Company { get; set; }

	}
}