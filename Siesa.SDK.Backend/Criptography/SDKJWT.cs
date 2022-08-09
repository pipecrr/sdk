using Siesa.SDK.Shared.Criptography;

namespace Siesa.SDK.Backend.Criptography
{
    public class SDKJWT : ISDKJWT
    {
        public SDKJWT()
        {
        }
        
        public string Generate<T>(T obj, long min)
        {
            return JWTUtils.Generate<T>(obj, SDKRsaKeys.PrivateKey, min);
        }

        public T Validate<T>(string token)
        {
            return JWTUtils.Validate<T>(token, Siesa.SDK.Shared.Criptography.SDKRsaKeys.PublicKey);
        }
    }
}