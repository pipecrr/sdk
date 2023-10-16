using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace Siesa.SDK.Entities
{

	public abstract partial class BaseMasterCompany<T, U> : BaseCompany<T>
	{
		

		public virtual U Id { get; set; }

		[SDKRequired]
		[SDKStringLength(250)]
		public virtual string Name { get; set; }

		[SDKStringLength(2000)]
		public virtual string? Description { get; set; }

		[SDKRequired]
		public virtual enumStatusBaseMaster Status { get; set; }

		[SDKRequired]
		public virtual bool IsPrivate { get; set; }

		public virtual int? RowidAttachment { get; set; }


	}
}