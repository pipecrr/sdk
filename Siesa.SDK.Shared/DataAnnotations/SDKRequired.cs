using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.DataAnnotations
{
    /// <summary>
    /// Specifies that a data field is required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class SDKRequired: RequiredAttribute 
    {
        /// <summary>
        /// Initializes a new instance of the SDKRequired class.
        /// </summary>
        public SDKRequired()
        {
            this.ErrorMessage = "Custom.Validator.FieldRequired";
        }

        /// <summary>
        /// Determines whether the specified value of the object is valid.
        /// </summary>
        /// <param name="value">The value of the object to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the ValidationResult class.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = base.IsValid(value, validationContext);
            if (validationResult == ValidationResult.Success)
            {
                return validationResult;
            }
            var modelTypeName = validationContext.ObjectType.Name;
            if(modelTypeName.EndsWith("DTO"))
            {
            
                modelTypeName = $"DTO.{modelTypeName.Substring(0, modelTypeName.Length - 3)}";
            }
            return new ValidationResult($"{validationResult.ErrorMessage}//{modelTypeName}.{ validationContext.MemberName }", new string[1] { validationContext.MemberName });
        }
    }
}