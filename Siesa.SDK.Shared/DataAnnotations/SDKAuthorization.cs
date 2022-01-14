using System;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SDKAuthorization : Attribute
    {
        //List of fields
        public string TableName { get; set; }

        //Constructor
        public SDKAuthorization(string table_name)
        {
            TableName = table_name;
        }

        public SDKAuthorization()
        {
            TableName = "";
        }
    }
}