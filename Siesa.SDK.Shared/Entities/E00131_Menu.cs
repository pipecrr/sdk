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

    public enum MenuType
    {
        Separator = 1,
        Menu = 2,
        Feature = 3
    }
    public class E00131_Menu
    {
        [Key]
        public int Rowid { get; set; }
        public string ID { get; set; }
        public bool IsInternal { get; set; }
        public MenuType Type { get; set; }
        public int? RowidMenuParent { get; set; }
        [ForeignKey(nameof(RowidMenuParent))]
        public virtual E00131_Menu MenuParent { get; set; }
        public int Level { get; set; } //TODO: Check if this is needed
        public int Order { get; set; }
        public int? RowidFeature { get; set; }
        [ForeignKey(nameof(RowidFeature))]
        public virtual E00105_Feature Feature { get; set; }
        public string Title { get; set; } //Todo: make this a localized string
        public string Description { get; set; } //Todo: make this a localized string
        public string Image { get; set; } //Fontawesome icon or image URL

        public virtual ICollection<E00131_Menu> SubMenus { get; set; }
	}
}
