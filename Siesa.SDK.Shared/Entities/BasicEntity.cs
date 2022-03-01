
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    [Index(nameof(ID), IsUnique = true)]
    public abstract class BasicEntity<T>: BaseEntity
    {
        public virtual string Description { get; set; }
        [Required]
        public virtual T ID { get; set; }
        public virtual string Notes { get; set; }

        public virtual bool Status { get; set; }

        public override string ToString()
        {
            return $"({ID}) - {Description}";
        }
    }
}
