using System;
using System.ComponentModel.DataAnnotations;
using Siesa.SDK.Shared.Criptography;

namespace Siesa.SDK.Shared.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class SDKDataEncrypt: RequiredAttribute 
    {
        public SDKDataEncrypt()
        {
            //parametrizar algoritmo de encriptacion (Enum)

        }

        public static string FieldEncrypt(string valueEncrypt)
        {
            var _salt = "HolaMundo"; //TODO: get from config -- AppSettings SDK?
            var encryptorPassword = new SDKEncryptSHA256(_salt);
            return encryptorPassword.Encrypt(valueEncrypt);
        }
        //traer el Metodo de encriptacion
    }
}