using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Entities
{
    public class E00105_Feature
    {
        [Key]
        public int Rowid { get; set; }

        public string Description { get; set; }
    }
}
