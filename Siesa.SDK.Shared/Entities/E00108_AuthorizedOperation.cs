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
    public class E00108_AuthorizedOperation: BaseAudit<int>
    {
        public virtual E00105_Feature Feature { get; set; }
        public virtual E00099_GroupCompany GroupCompany { get; set; }
        public virtual E00103_Role Role { get; set; }
        public int? RowidUser { get; set; }
        [ForeignKey(nameof(RowidUser))]
        public virtual E00102_User User { get; set; }
        public string Value { get; set; }

	}
}
