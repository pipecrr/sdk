using System;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SDKExternalView : Attribute
    {
        public bool isEmptyLayout { get; set; }
        public SDKExternalView(bool isEmptyLayout = false)
        {
            this.isEmptyLayout = isEmptyLayout;
        }
    }
}