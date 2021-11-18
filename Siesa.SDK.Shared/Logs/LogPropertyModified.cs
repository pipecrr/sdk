using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Log
{
    public class LogPropertyModified : LogProperty
    {
        public string CurrentValue { get; set; }
        public string OldValue { get; set; }
    }
}
