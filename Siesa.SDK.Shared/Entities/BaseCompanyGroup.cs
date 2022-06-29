using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseCompanyGroup<T> : BaseAudit<T>
	{
		

		[Required]
		public virtual short RowidCompanyGroup { get; set; }


	}
}