using Siesa.SDK.Shared.Criptography;
using Siesa.SDK.Shared.Services;
using System.Collections.Generic;

namespace Siesa.SDK.Backend.Criptography
{
    public class SDKJWT : ISDKJWT
    {
        public SDKJWT()
        {
        }
        
        public string Generate<T>(T obj, long min, Dictionary<string, List<int>>? featurePermissions = null, List<SessionRol> roles = null,    short rowIdDBConnection = 0, short rowidcompanygroup =0)
        {
            return JWTUtils.Generate<T>(obj, SDKRsaKeys.PrivateKey, min, featurePermissions, roles, rowIdDBConnection, rowidcompanygroup);
        }

        public T Validate<T>(string token)
        {
            return JWTUtils.Validate<T>(token, Siesa.SDK.Shared.Criptography.SDKRsaKeys.PublicKey);
        }
    }
}