using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Entities.Fields
{
    [Owned]
    public class TextFieldEntity
    {
        public string Notes { get; set; }
    }
}
