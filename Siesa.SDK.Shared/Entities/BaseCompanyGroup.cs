using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Entities.Enums;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseCompanyGroup<T> : BaseAudit<T>
	{
		

		[ForeignKey("CompanyGroup")]
		[Required]
		public virtual short RowidCompanyGroup { get; set; }


		[Required]
		public virtual E00200_CompanyGroup CompanyGroup { get; set; }

	}
}