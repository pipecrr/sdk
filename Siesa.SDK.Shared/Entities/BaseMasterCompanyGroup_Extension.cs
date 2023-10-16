using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
	public abstract partial class BaseMasterCompanyGroup<T, U> : BaseCompanyGroup<T>
	{
		public override string ToString()
        {
            return $"({Id}) - {Name}";
        }
	}
}