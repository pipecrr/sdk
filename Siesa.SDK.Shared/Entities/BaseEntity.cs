
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Siesa.SDK.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int RowID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
            
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)] TODO: Revisar, la db genera un nulo
        public DateTime? LastUpdateDate { get; set; } = DateTime.UtcNow;

        //source of data
        public virtual string Source { get; set; }

        //Todo: Owner: foreign key with User, LastEditUser, Usuario creador and Owner_team
        [Timestamp]
        public virtual byte[] RowVersion { get; set; }

    }
}
