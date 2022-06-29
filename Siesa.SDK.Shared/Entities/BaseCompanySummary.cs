using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseCompanySummary : BaseSDK<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		[ForeignKey("Company")]
		[Required]
		public virtual short RowidCompany { get; set; }


		[Required]
		public virtual E00201_Company Company { get; set; }

	}
}