using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Validators
{
    public interface IBLBaseValidator<T,K> 
    {

    }
    public class BLBaseValidator<T> : AbstractValidator<T>
    {
        public string ValidatorType { get; set; }
    }
}
