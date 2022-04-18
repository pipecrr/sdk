
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public abstract class BaseCompanySummary<T>: BaseSDK<T>
    {
        public int? RowidCompany { get; set; }
        [ForeignKey(nameof(RowidCompany))]
        public virtual E00200_GroupCompany Company { get; set; }
    }
}
