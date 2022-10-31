using System;
using System.ComponentModel.DataAnnotations;
using Siesa.SDK.Shared.Criptography;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class SDKDataEncrypt: Attribute 
    {
        public SDKDataEncrypt()
        {
            //parametrizar algoritmo de encriptacion (Enum)
        }

        public static string FieldEncrypt(object valueEncrypt)
        {
            var _salt = "hola"; //TODO: get from config
            var encryptorPassword = new SDKEncryptSHA256(_salt);
            if (valueEncrypt != null)
            {
                return encryptorPassword.Encrypt(valueEncrypt.ToString());
            }
            else
            {
                return null;
            }
        }
    }
}