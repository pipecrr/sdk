using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Entities.Enums;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseCompany<T> : BaseAudit<T>
	{
		

		[ForeignKey("Company")]
		[Required]
		public virtual short RowidCompany { get; set; }


		[Required]
		public virtual E00201_Company Company { get; set; }

	}
}