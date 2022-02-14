using Microsoft.EntityFrameworkCore;
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
    //[SDKAuthorization]
    public class E00130_MenuGroup
    {
        [Key]
        public int Rowid { get; set; }
        public string ID { get; set; }
        public bool IsInternal { get; set; }
        public string Description { get; set; } //Todo: make this a localized string
        public int Order { get; set; } //TODO: Check if this is needed
        public string Image { get; set; } //Fontawesome icon or image URL
        public virtual E00102_User Creator { get; set; }
	}
}
