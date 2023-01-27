using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.DataAnnotations;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseCompanyGroup<T> : BaseAudit<T>
	{
		

		[ForeignKey("CompanyGroup")]
		[Required]
		public virtual short RowidCompanyGroup { get; set; }


		[SDKCheckRelationship]
		public virtual E00200_CompanyGroup CompanyGroup { get; set; }

	}
}