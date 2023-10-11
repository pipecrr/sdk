using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace Siesa.SDK.Entities
{

	public abstract partial class BaseCompanyTransaction<T> : BaseCompany<T>
	{
		

		public virtual int? RowidAttachment { get; set; }


	}
}