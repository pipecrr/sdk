
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    [Index(nameof(ID), IsUnique = true)]
    public abstract class BaseMasterCompanyGroup<T,K>: BaseCompanyGroup<T>
    {
        [StringLength(2000)]
        public virtual string Description { get; set; }
        [StringLength(250)]
        public virtual string Name { get; set; }
        [Required]
        public virtual K ID { get; set; }

        public virtual bool Status { get; set; }
        public virtual bool IsPrivate { get; set; }

        //TODO: Agregar relación con attachment

        public override string ToString()
        {
            return $"({ID}) - {Description}";
        }
    }
}
