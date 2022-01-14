﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Entities
{
    public class E00099_GroupCompany: BasicEntity<string>
    {
        [JsonIgnore]
        public virtual ICollection<E00101_CompaniesGroups> Companies { get; set; }
    }
}
