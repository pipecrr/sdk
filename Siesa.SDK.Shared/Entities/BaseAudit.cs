
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public abstract class BaseAudit<T>: BaseSDK<T>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
            
        public DateTime? LastUpdateDate { get; set; } = DateTime.UtcNow;
        public virtual string? Source { get; set; }
        public int? RowidUserCreates { get; set; }
        [ForeignKey(nameof(RowidUserCreates))]
        public virtual E00220_User UserCreates { get; set; }

        public int? RowidUserLastUpdate { get; set; }
        [ForeignKey(nameof(RowidUserLastUpdate))]
        public virtual E00220_User UserLastUpdate { get; set; }

        public int? RowidSession { get; set; } //TODO: Agregar relación con entidad
        

    }
}
