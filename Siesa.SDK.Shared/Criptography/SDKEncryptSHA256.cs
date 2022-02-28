using System.Security.Cryptography;
using System.Text;
using System;

namespace Siesa.SDK.Shared.Criptography
{
    public class SDKEncryptSHA256 : ISDKEncrypt
    {
        private readonly string _salt;

        private SDKEncryptSHA256() : this(string.Empty) {  }
        public SDKEncryptSHA256(string salt)
        {
            _salt = salt;
        }
        public string Encrypt(string text)
        {
            using (var sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text + _salt));
                return ConvertByteArrayToString(bytes);
            }
            
        }

        private static string ConvertByteArrayToString(byte[] bytes)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public bool Compare(string password, string encryptText)
        {
            if(string.IsNullOrEmpty(password) || string.IsNullOrEmpty(encryptText))
            {
                return false;
            }else
            {
                return encryptText.Equals(Encrypt(password));
            }
        }

        
    }
}
