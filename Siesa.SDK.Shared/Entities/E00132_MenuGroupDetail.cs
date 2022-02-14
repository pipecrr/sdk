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
    public class E00132_MenuGroupDetail
    {
        [Key]
        public int Rowid { get; set; }
        public virtual E00130_MenuGroup MenuGroup {get;set;}
        public virtual E00131_Menu Menu {get;set;}
        public virtual E00102_User Creator { get; set; }
	}
}
