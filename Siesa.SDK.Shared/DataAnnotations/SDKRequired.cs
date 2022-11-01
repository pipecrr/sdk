using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class SDKRequired: RequiredAttribute 
    {
        public SDKRequired()
        {
            this.ErrorMessage = "Custom.Validator.FieldRequired";
        }
    }
}
