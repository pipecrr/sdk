using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class SDKStringLength: StringLengthAttribute 
    {
        public SDKStringLength(int maximumLength): base(maximumLength) 
        {
            this.ErrorMessage = "Custom.Validator.FieldLength";
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = base.IsValid(value, validationContext);
            if (validationResult == ValidationResult.Success)
            {
                return validationResult;
            }

            if (this.MinimumLength != 0)
            {
                this.ErrorMessage = "Custom.Validator.FieldLengthRange";
                return new ValidationResult($"{validationResult.ErrorMessage}//{validationContext.ObjectType.Name}.{validationContext.MemberName}//{this.MaximumLength}//{this.MinimumLength}", new string[1] { validationContext.MemberName });
            }  
            
            return new ValidationResult($"{validationResult.ErrorMessage}//{validationContext.ObjectType.Name}.{validationContext.MemberName}//{this.MaximumLength}", new string[1] { validationContext.MemberName });
            

            
        }
    }
}