using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Results
{
    public class BaseOperationResult
    {
        private ICollection<IOperationError> operationErrors;
        public bool Succesfull
        {
            get
            {
                return operationErrors.Count() == 0;
            }
        }

        public ICollection<IOperationError> Errors
        {
            get
            {
                return operationErrors;
            }
        }

        public BaseOperationResult()
        {
            operationErrors = new List<IOperationError>();
        }

        public void AddOperationError(IOperationError operationError)
        {
            operationErrors.Add(operationError);
        }

        public string Resume()
        {
            return Resume(string.Empty, "-", string.Empty);
        }

        public string Resume(string initial, string middle, string end)
        {
            var stringBuilder = new StringBuilder();
            foreach (var error in operationErrors)
            {
                stringBuilder.AppendLine($"{initial} {error.GetAttribute()} {middle} {error.GetMessage()} {end}");
            }
            return stringBuilder.ToString();
        }

        public string ResumeHTML()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("<ul>");
            stringBuilder.AppendLine(Resume("<li>", "-", "</li>"));
            stringBuilder.AppendLine("</ul>");
            return stringBuilder.ToString();
        }
    }
}
