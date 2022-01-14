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
    public class E00103_Role: BasicEntity<string>
    {
        public virtual ICollection<E00099_GroupCompany> GroupCompany { get; set; }
        public virtual ICollection<E00104_UsersRoles> Users { get; set; }

        public virtual ICollection<E00108_AuthorizedOperation> Operation_Permissions { get; set; }
    }
}
