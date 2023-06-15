using System;
using System.Collections.Generic;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SDKExposedMethod : Attribute
    {

        public string[] Permissions { get; set; }
        public SDKExposedMethod(string[] Permissions)
        {
            this.Permissions = Permissions;
        }
        public SDKExposedMethod()
        {
            Permissions = new string[] {};
        }
    }
}