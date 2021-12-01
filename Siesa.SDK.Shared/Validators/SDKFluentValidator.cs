using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Siesa.SDK.Protos;

namespace Siesa.SDK.Shared.Validators
{
    public static class SDKFluentValidator
    {
        public static bool Validate<T>(BLBaseValidator<T> validator, T BaseObject, ref ValidateAndSaveBusinessObjResponse operationResult)
        {
            ValidationResult result = validator.Validate(BaseObject);

            foreach (var resultFluent in result.Errors)
            {
                operationResult.Errors.Add(new OperationError
                {
                    Attribute = (validator.ValidatorType != "" ? validator.ValidatorType + "." : "") + resultFluent.PropertyName,
                    Message = resultFluent.ErrorMessage
                });
            }

            return true;
        }
    }
}
