using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Validators
{
    public interface IBLBaseValidator : IValidator
    {
        string ValidatorType { get; set; }
    }

    public class BLBaseValidator<T> : AbstractValidator<T>, IBLBaseValidator
    {
        public string ValidatorType { get; set; }
    }
}
