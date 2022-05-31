using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SDKCheckRelationship: Attribute 
    {
        public SDKCheckRelationship()
        {
        }
    }
}
