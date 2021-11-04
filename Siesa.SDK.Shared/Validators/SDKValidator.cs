using Siesa.SDK.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Validators
{
    public class SDKValidator
    {

        public static void Validate<T>(T obj, BLBaseValidator<T> validator,  ref BaseOperationResult operationResult)
        {
            SDKDataAnnotationValidator.Validate<T>(obj, ref operationResult);
            SDKFluentValidator.Validate<T>(validator, obj, ref operationResult);
        }
    }
}
