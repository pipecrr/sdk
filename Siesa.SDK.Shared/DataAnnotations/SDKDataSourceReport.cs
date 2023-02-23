using System;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SDKDataSourceReport : Attribute
    {
        public SDKDataSourceReport()
        {
        }
    }
}