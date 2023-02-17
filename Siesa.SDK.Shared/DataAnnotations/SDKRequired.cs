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

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = base.IsValid(value, validationContext);
            if (validationResult == ValidationResult.Success)
            {
                return validationResult;
            }
            return new ValidationResult($"{validationResult.ErrorMessage}//{validationContext.ObjectType.Name}.{ validationContext.MemberName }", new string[1] { validationContext.MemberName });
        }
    }
}
