using Siesa.SDK.Protos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Validators
{
    public static class SDKDataAnnotationValidator
    {
        public static bool Validate<T>(T obj, ref ValidateAndSaveBusinessObjResponse operationResult)
        {
            var resultsDataAnnotation = new List<ValidationResult>();            

            var validate = Validator.TryValidateObject(obj, new ValidationContext(obj), resultsDataAnnotation, true);


            foreach (var resultDataAnnotation in resultsDataAnnotation)
            {
                operationResult.Errors.Add(new OperationError { 
                    Attribute = resultDataAnnotation.MemberNames.FirstOrDefault(),
                    Message = resultDataAnnotation.ErrorMessage
                });
            }

            return validate;
        }
    }
}
