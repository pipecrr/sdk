using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Backend.Exceptions
{
    public static class BackendExceptionManager
    {
        public static string ExceptionToString(DbUpdateException exception)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("Exception updating record(s) in the database: ");
            if (exception.InnerException != null)
            {
                messageBuilder.AppendLine(ExceptionToString((Microsoft.Data.SqlClient.SqlException) exception.InnerException));
            }
            return messageBuilder.ToString();
        }

        private static string ExceptionToString(Microsoft.Data.SqlClient.SqlException exception)
        {
            var messageBuilder = new StringBuilder();
            foreach(Microsoft.Data.SqlClient.SqlError error in exception.Errors)
            {
                messageBuilder.AppendLine(ErrorToString(error));
            }
            return messageBuilder.ToString();
        }
        private static string ErrorToString(Microsoft.Data.SqlClient.SqlError error)
        {
            if(error.Number == 547)
            {
                return "Error to insert record";
            }
            if(error.Number == 2601)
            {
                return error.Message;
            }
            if (error.Number == 3621)
            {
                return string.Empty;
            }
            return error.Message;

        }
    }
}
