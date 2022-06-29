using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
	public abstract partial class BaseDynamicEntity<T, U> : BaseAudit<int>
	{


	}
}