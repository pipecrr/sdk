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
    public class E00133_FavoritesMenu
    {
        [Key]
        public int Rowid { get; set; }

        public int? RowidUser { get; set; }
        [ForeignKey(nameof(RowidUser))]
        public virtual E00102_User User { get; set; }

        public int? RowidMenu { get; set; }
        [ForeignKey(nameof(RowidMenu))]
        public virtual E00131_Menu Menu { get; set; }

        public string Description { get; set; }
        public DateTime? LastUseDate { get; set; }
	}
}
