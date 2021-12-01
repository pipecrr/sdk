namespace Siesa.SDK.Shared.Criptography
{
    public interface ISDKEncrypt
    {
        public string Encrypt(string text);
        public bool Compare(string text, string encryptText);
    }
}