using System;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SDKDashlet : Attribute
    {
        public string ResourceTag { get; set; }
        public Type PreviewComponentType { get; set; }
        public SDKDashlet(string ResourceTag, Type PreviewComponentType = null)
        {
            this.ResourceTag = ResourceTag;
            this.PreviewComponentType = PreviewComponentType;
        }
    }
}