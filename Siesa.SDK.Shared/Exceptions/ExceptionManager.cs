using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Exceptions
{
    public static class ExceptionManager
    {
        public static string ExceptionToString(Exception exception)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("Uncontroled exception: ");
            messageBuilder.AppendLine($"Type exception: {exception.GetType().FullName}");
            messageBuilder.AppendLine($"Message: {exception.Message}");
            if (exception.InnerException != null)
                messageBuilder.AppendLine($"InnerExpetion: {exception.InnerException.Message}");
            return messageBuilder.ToString();
        }

        
    }
}
