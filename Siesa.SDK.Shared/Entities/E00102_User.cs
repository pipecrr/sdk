using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Siesa.SDK.Shared.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Entities
{
    [Index(nameof(UserName), IsUnique = true)]
    [SDKAuthorization]
    [SDKLogEntity]
    public class E00102_User: BaseEntity
    {
        [Required]
        public string UserName { get; set; }
        public string Alias { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public DateTime PasswordUpdateDate { get; set; }
        public DateTime LastValidLogin { get; set; }
        public DateTime LastUnvalidLogin { get; set; }

        [JsonIgnore]
        public virtual ICollection<E00111_UsersTeams> Teams { get; set; }

        [JsonIgnore]
        public virtual ICollection<E00104_UsersRoles> Roles { get; set; }

        

        //public virtual ICollection<S010_Operation_Permission> Operation_Permissions { get; set; }
        //public virtual ICollection<S011_Operation_Override> Operation_Override { get; set; }

        public int? RowidReportsTo { get; set; }
        [ForeignKey(nameof(RowidReportsTo))]
        [InverseProperty(nameof(Employees))]
        public virtual E00102_User ReportsTo { get; set; }

        [InverseProperty(nameof(ReportsTo))]

        [JsonIgnore]
        public virtual ICollection<E00102_User> Employees { get; set; }

        public override string ToString()
        {
            return $"{UserName}";
        }
    }
}
