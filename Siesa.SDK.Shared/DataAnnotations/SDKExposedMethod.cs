using System;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SDKExposedMethod : Attribute
    {
        public SDKExposedMethod()
        {
        }
    }
}