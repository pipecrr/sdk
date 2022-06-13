using Microsoft.EntityFrameworkCore;
using Siesa.SDK.Backend.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Backend.Exceptions
{
    public static class BackendExceptionManager
    {
        public static string ExceptionToString(DbUpdateException exception, SDKContext dbContext)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("Exception updating record(s) in the database: ");
            if (exception.InnerException != null)
            {
                if(exception.InnerException is Microsoft.Data.SqlClient.SqlException)
                {
                    messageBuilder.Append(ExceptionToStringSQL((Microsoft.Data.SqlClient.SqlException) exception.InnerException, dbContext));
                }else if(exception.InnerException is Npgsql.PostgresException)
                {
                    messageBuilder.Append(ExceptionToPostgreSQL((Npgsql.PostgresException) exception.InnerException, dbContext));
                }
                
            }
            return messageBuilder.ToString();
        }

        private static string ExceptionToPostgreSQL(Npgsql.PostgresException exception, SDKContext dbContext)
        {
            return exception.Message;
        }

        private static string ExceptionToStringSQL(Microsoft.Data.SqlClient.SqlException exception, SDKContext dbContext)
        { 
            string errorMessage = string.Empty;
            var messageBuilder = new StringBuilder();
            foreach(Microsoft.Data.SqlClient.SqlError error in exception.Errors)
            {
                errorMessage = DBErrorToString(error, dbContext);
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    messageBuilder.AppendLine(errorMessage);
                }                
            }
            return messageBuilder.ToString();
        }
        private static string DBErrorToString(Microsoft.Data.SqlClient.SqlError error, SDKContext dbContext)
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
                if(dbContext != null){
                    //get index by table name and index name
                    var tableName = match?.Groups["TableName"].Value;
                    //remove "dbo." if exists
                    if (tableName != null && tableName.StartsWith("dbo."))
                    {
                        tableName = tableName.Substring(4);
                    }
                    var entityType = dbContext.Model.GetEntityTypes().FirstOrDefault(x => x.GetTableName() == tableName);
                    if (entityType != null)
                    {
                        var index = entityType.GetIndexes().Where(x => x.Name == match?.Groups["IndexName"].Value).FirstOrDefault();
                        if(index != null){
                            //get properties of the index
                            var properties = index.Properties.Select(x => x.Name).ToList();
                            message += $"\nProperties: { string.Join(", ", properties)}";

                            message += $"\n\nLos campos { string.Join(", ", properties)} deben ser únicos.";
                        }
                    }
                    
                }

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
