﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Entities
{
    public class E00109_OverwriteAuthorizedOperation: BasicEntity<string>
    {
        public virtual E00107_Operation Feature_Operation { get; set; }
        public virtual E00100_Company Company { get; set; }
        public int? RowidUser { get; set; }
        [ForeignKey(nameof(RowidUser))]
        public virtual E00102_User User { get; set; }
        public string Value { get; set; }

	}
}