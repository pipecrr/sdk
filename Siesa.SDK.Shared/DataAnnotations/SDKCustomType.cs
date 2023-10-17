using System;
using System.ComponentModel.DataAnnotations;
using Siesa.Global.Enums;
using Siesa.SDK.Shared.Criptography;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class SDKCustomType: Attribute 
    {
        public EnumCustomTypeField _customType {get; set;}
        public SDKCustomType(EnumCustomTypeField customType)
        {
            _customType = customType;
        }

    }
}