
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public abstract class BaseCompanyGroup<T>: BaseAudit<T>
    {
        public short? RowidCompanyGroup { get; set; }
        [ForeignKey(nameof(RowidCompanyGroup))]
        public virtual E00200_GroupCompany CompanyGroup { get; set; }


    }
}
