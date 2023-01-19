using System;
using System.Collections.Generic;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SDKExposedMethod : Attribute
    {

        public int[] Permissions { get; set; }
        public SDKExposedMethod(int[] Permissions)
        {
            this.Permissions = Permissions;
        }
        public SDKExposedMethod()
        {
            Permissions = new int[] {};
        }
    }
}