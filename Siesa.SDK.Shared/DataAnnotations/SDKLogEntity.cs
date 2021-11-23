using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SDKLogEntity: Attribute 
    {
        //List of fields
        public string[] Fields { get; set; }

        //Constructor
        public SDKLogEntity(string[] fields)
        {
            Fields = fields;
        }

        public SDKLogEntity()
        {
            Fields = new string[] { };
        }
        
    }
}
