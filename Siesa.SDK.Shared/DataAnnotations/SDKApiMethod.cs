using System;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SDKApiMethod : Attribute
    {
        public string HTTPMethod { get; set; } = "GET";

        //Constructor
        public SDKApiMethod(string http_method)
        {
            HTTPMethod = http_method;
        }

        public SDKApiMethod()
        {
            HTTPMethod = "GET";
        }
    }
}