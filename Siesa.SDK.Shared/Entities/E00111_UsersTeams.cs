using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Entities
{
    public class E00111_UsersTeams
    {
        [Key]
        public int Rowid { get; set; }
        public virtual E00102_User User { get; set; }
        public virtual E00110_Team Team { get; set; }
	}
}
