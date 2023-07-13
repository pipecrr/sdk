using Siesa.SDK.Shared.Criptography;
using System.Collections.Generic;
using Siesa.SDK.Shared.Services;

namespace Siesa.SDK.Frontend.Criptography
{
    public class SDKJWT : ISDKJWT
    {
        public SDKJWT()
        {
        }
        
        public string Generate<T>(T obj, long min)
        {
            throw new System.NotImplementedException();
        }

        public T Validate<T>(string token)
        {
            return JWTUtils.Validate<T>(token, SDKRsaKeys.PublicKey);
        }
    }
}