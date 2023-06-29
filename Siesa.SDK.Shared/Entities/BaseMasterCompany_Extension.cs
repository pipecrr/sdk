using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
	public abstract partial class BaseMasterCompany<T> : BaseCompany<int>
	{
		public override string ToString()
        {
            return $"{Id} - {Name}";
        }

	}
}