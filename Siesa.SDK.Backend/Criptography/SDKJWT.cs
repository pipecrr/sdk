using Siesa.SDK.Shared.Criptography;
using Siesa.SDK.Shared.Services;
using System.Collections.Generic;
using Siesa.SDK.Entities;
using System.IdentityModel.Tokens.Jwt;

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

        public JwtSecurityToken RenewToken(string token)
        {
            return JWTUtils.RenewToken(token, SDKRsaKeys.PrivateKey);
        }
    }
}