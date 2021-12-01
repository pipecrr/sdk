using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Entities
{
    [Index(nameof(UserName), IsUnique = true)]
    public class S004_User: BaseEntity
    {
        [Required]
        public string UserName { get; set; }
        public string Alias { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public string CreationUser { get; set; }
        public string UpdateUser { get; set; }
        public DateTime PasswordUpdateDate { get; set; }
        public DateTime LastValidLogin { get; set; }
        public DateTime LastUnvalidLogin { get; set; }

	}
}
