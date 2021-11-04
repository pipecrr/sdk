using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Siesa.SDK.Shared.Results;

namespace Siesa.SDK.Shared.Validators
{
    public static class SDKFluentValidator
    {
        public static bool Validate<T>(BLBaseValidator<T> validator, T BaseObject, ref BaseOperationResult operationResult)
        {
            ValidationResult result = validator.Validate(BaseObject);

            foreach (var resultFluent in result.Errors)
            {
                operationResult.AddOperationError(new BasicOperationError(resultFluent.PropertyName, resultFluent.ErrorMessage));                
            }

            return true;
        }
    }
}
