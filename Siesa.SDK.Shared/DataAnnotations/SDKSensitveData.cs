using System;
using System.ComponentModel.DataAnnotations;
using Siesa.SDK.Shared.Criptography;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class SDKSensitiveData: Attribute 
    {
        public SDKSensitiveData()
        {
        }
    }
}