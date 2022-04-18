
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public abstract class BaseAdditionalData<T, K>: BaseAudit<T>
    {
        public int? RowidRecord { get; set; }
        [ForeignKey(nameof(RowidRecord))]
        public virtual K Record { get; set; }

        public int? RowidCompanyGroup { get; set; }
        [ForeignKey(nameof(RowidCompanyGroup))]
        public virtual E00200_GroupCompany CompanyGroup { get; set; }

        public int? RowidCompany { get; set; }
        [ForeignKey(nameof(RowidCompany))]
        public virtual E00201_Company Company { get; set; }
        //TODO: Agregar relación con attachment
    }
}
