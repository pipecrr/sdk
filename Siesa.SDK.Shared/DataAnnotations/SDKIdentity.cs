using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SDKIdentity: Attribute 
    {
        public bool Manual { get; set; }
        public SDKIdentity()
        {
        }

        public SDKIdentity(bool manual)
        {
            Manual = manual;
        }
    }
}
