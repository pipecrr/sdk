using System.Collections.Generic;
using Siesa.SDK.Shared.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Siesa.SDK.Shared.Criptography
{
    public interface ISDKJWT
    {
        public string Generate<T>(T obj, long min);

        public T Validate<T>(string token);

        public JwtSecurityToken RenewToken(string token);
    }
}