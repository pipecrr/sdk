using System;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SDKExternalView : Attribute
    {
        public SDKExternalView()
        {
        }
    }
}