
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Rowid { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
            
        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;

        //source of data
        public virtual string Source { get; set; }

        [Timestamp]
        public virtual byte[] RowVersion { get; set; }

        //[Required]
        public int? RowidCreator { get; set; }
        [ForeignKey(nameof(RowidCreator))]
        public virtual E00102_User Creator { get; set; }

        public int? RowidLastEditUser { get; set; }
        [ForeignKey(nameof(RowidLastEditUser))]
        public virtual E00102_User LastEditUser { get; set; }

    }
}
