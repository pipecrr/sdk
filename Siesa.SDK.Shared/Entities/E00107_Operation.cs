using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Entities
{
    public class E00107_Operation
    {
        [Key]
        public int Rowid { get; set; }

        public virtual E00105_Feature Feature { get; set; }
        public virtual E00106_Action Action { get; set; }
    }
}
