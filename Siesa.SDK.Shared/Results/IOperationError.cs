using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Results
{
    public interface IOperationError
    {
        public string GetMessage();

        public string GetAttribute()
        {
            return string.Empty;
        }
    }
}
