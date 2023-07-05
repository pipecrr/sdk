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

                string ValueMessage = resultFluent.ErrorMessage;

                if (resultFluent.ErrorMessage.Split("//").Count() > 1)
                {
                    var splitMessage = ValueMessage.Split("//");

                    ValueMessage = $"{splitMessage[0]}//{BaseObject.GetType().Name}.{splitMessage[1]}";
                    
                }else if (!String.IsNullOrEmpty(resultFluent.PropertyName))
                {

                    ValueMessage = $"{resultFluent.ErrorMessage}//{BaseObject.GetType().Name}.{resultFluent.PropertyName}";

                }
                // else if (resultFluent.ErrorMessage.Split("//").Count() > 1)
                // {
                //     var splitMessage = ValueMessage.Split("//");

                //     ValueMessage = $"{splitMessage[0]}//{BaseObject.GetType().Name}.{splitMessage[1]}";
                // }

                operationResult.Errors.Add(new OperationError
                {
                    Attribute = (validator.ValidatorType != "" ? validator.ValidatorType + "." : "") + resultFluent.PropertyName,

                   
                    Message = ValueMessage
                });
            }

            return true;
        }
    }
}
