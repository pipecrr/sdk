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
                messageBuilder.Append(ExceptionToString((Microsoft.Data.SqlClient.SqlException) exception.InnerException));
            }
            return messageBuilder.ToString();
        }

        private static string ExceptionToString(Microsoft.Data.SqlClient.SqlException exception)
        { 
            string errorMessage = string.Empty;
            var messageBuilder = new StringBuilder();
            foreach(Microsoft.Data.SqlClient.SqlError error in exception.Errors)
            {
                errorMessage = ErrorToString(error);
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    messageBuilder.AppendLine(errorMessage);
                }                
            }
            return messageBuilder.ToString();
        }
        private static string ErrorToString(Microsoft.Data.SqlClient.SqlError error)
        {
            string message = string.Empty;

            if (error.Number == 547)
            {
                var errorMessage = error.Message.Replace("\"","'");
                var regex = @"\A(\s?\w+\s?)*\'(?<ForeingKey>.+?)\'\.(\s?\w+\s?)*\'(?<DataBase>.+?)\'\,(\s?\w+\s?)*\'(?<TableName>.+?)\'\,(\s?\w+\s?)*\'(?<ColumnName>.+?)\'.";
                var match = new System.Text.RegularExpressions.Regex(regex, System.Text.RegularExpressions.RegexOptions.Compiled).Match(errorMessage);

                message += $"Foreing key: { match?.Groups["ForeingKey"].Value}";
                message += $"\nData base: { match?.Groups["DataBase"].Value}";
                message += $"\nTable name: { match?.Groups["TableName"].Value}";
                message += $"\nColumn name: { match?.Groups["ColumnName"].Value}";

                return message;
            }

            if(error.Number == 2601)
            { 
                var regex = @"\A(\s?\w+\s?)*\'(?<TableName>.+?)\'(\s?\w+\s?)*\'(?<IndexName>.+?)\'\.(\s?\w+\s?)*\((?<KeyValues>.+?)\)";
                var match = new System.Text.RegularExpressions.Regex(regex, System.Text.RegularExpressions.RegexOptions.Compiled).Match(error.Message);
            
                message += $"Table: { match?.Groups["TableName"].Value}";
                message += $"\nIndex name: { match?.Groups["IndexName"].Value}";
                message += $"\nKey values: { match?.Groups["KeyValues"].Value}";

                return message;
            }
            
            if (error.Number == 3621)
            {
                return string.Empty;
            }
            return error.Message;

        }
    }
}
