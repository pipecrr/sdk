using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.SDK.Entities.Enums;

namespace Siesa.SDK.Entities
{

	public abstract partial class BaseMasterCompany<T> : BaseCompany<int>
	{
		[Key]
		[Required]
		public override int Rowid { get; set; }

		public virtual T Id { get; set; }

		[Required]
		[StringLength(250)]
		public virtual string Name { get; set; }

		[Required]
		[StringLength(2000)]
		public virtual string Description { get; set; }

		[Required]
		public virtual bool Status { get; set; }

		[Required]
		public virtual bool IsPrivate { get; set; }

		public virtual int? RowidAttachment { get; set; }


	}
}