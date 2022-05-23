
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public abstract class BaseCompany<T>: BaseAudit<T>
    {
        public Int16? RowidCompany { get; set; }
        [ForeignKey(nameof(RowidCompany))]
        public virtual E00201_Company Company { get; set; }


    }
}
