using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Results
{
    public class BasicOperationError : IOperationError
    {
        private readonly string _attribute;
        private readonly string _message;

        public BasicOperationError(string attribute, string message)
        {
            _attribute = attribute;
            _message = message;
        }

        public string GetMessage()
        {
            return _message;
        }

        public string GetAttribute()
        {
            return _attribute;
        }
    }
}
