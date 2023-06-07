using Siesa.SDK.Shared.Criptography;
using Siesa.SDK.Shared.Services;
using System.Collections.Generic;
using Siesa.SDK.Entities;

namespace Siesa.SDK.Backend.Criptography
{
    public class SDKJWT : ISDKJWT
    {
        public SDKJWT()
        {
        }
        
        public string Generate<T>(T obj, long min, Dictionary<string, List<int>>? featurePermissions = null, List<SessionRol> roles = null,    short rowIdDBConnection = 0, short rowidcompanygroup =0)
        {
            if(obj is E00220_User)
            {
                E00220_User user = (E00220_User)(object)obj;
                return JWTUtils.Generate(user, SDKRsaKeys.PrivateKey, min, featurePermissions, roles, rowIdDBConnection, rowidcompanygroup);
            }else
            {
                return JWTUtils.Generate<T>(obj, SDKRsaKeys.PrivateKey, min);
            }
        }

        public T Validate<T>(string token)
        {
            return JWTUtils.Validate<T>(token, Siesa.SDK.Shared.Criptography.SDKRsaKeys.PublicKey);
        }
    }
}