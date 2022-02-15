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
    public class E00101_CompaniesGroups
    {
        [Key]
        public int Rowid { get; set; }
        public virtual E00100_Company Company { get; set; }
        public virtual E00099_GroupCompany GroupCompany { get; set; }
    }
}
