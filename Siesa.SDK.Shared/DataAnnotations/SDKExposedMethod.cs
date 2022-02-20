using System;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SDKExposedMethod : Attribute
    {
        //List of fields
        public string TableName { get; set; }

        //Constructor
        public SDKExposedMethod(string table_name)
        {
            TableName = table_name;
        }

        public SDKExposedMethod()
        {
            TableName = "";
        }
    }
}