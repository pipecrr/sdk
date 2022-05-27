
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public abstract class BaseMaster<T>: BaseAudit<int>
    {

        [Required]
        [StringLength(2000)]
        public virtual string Description { get; set; }
        
        [Required]
        [StringLength(250)]
        public virtual string Name { get; set; }
        [Required]
        public virtual T Id { get; set; }

        public virtual bool Status { get; set; }
        public virtual bool IsPrivate { get; set; }

        //TODO: Agregar relación con attachment

        public override string ToString()
        {
            return $"({Id}) - {Name}";
        }
    }
}
