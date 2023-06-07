
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Services;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;


namespace Siesa.SDK.Shared.Criptography
{
    public static class JWTUtils
    {
        public static string Generate<T>(T data, string secretKey, long minutesExp)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = KeyHelper.BuildRsaSigningKey(secretKey);

            var serializerSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("data", Newtonsoft.Json.JsonConvert.SerializeObject(data,Formatting.None,serializerSettings)),
                }),
                Expires = DateTime.UtcNow.AddMinutes(minutesExp),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string Generate (E00220_User? user, string secretKey, long minutesExp, Dictionary<string, List<int>>? featurePermissions = null, List<SessionRol> roles = null,    short rowIdDBConnection = 0, short rowidcompanygroup =0)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = KeyHelper.BuildRsaSigningKey(secretKey);

            var serializerSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var userToken = new JwtUserData() {
                Rowid = user.Rowid,
                Path = user.Path,
                PasswordRecoveryEmail = user.PasswordRecoveryEmail,
                Name = user.Name,
                Id = user.Id,
                Description = user.Description,
                RowidCulture = user.RowidCulture,
                Roles = roles,
                FeaturePermissions = featurePermissions,
                RowidCompanyGroup = rowidcompanygroup,
                RowIdDBConnection = rowIdDBConnection,
                IsAdministrator = user.IsAdministrator,
                RowidAttachmentUserProfilePicture = user.RowidAttachmentUserProfilePicture
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("data", Newtonsoft.Json.JsonConvert.SerializeObject(userToken, Formatting.None, serializerSettings)),
                }),
                Expires = DateTime.UtcNow.AddMinutes(minutesExp),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static T Validate<T>(string token, string publicKey)
        {
            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(publicKey))
            {
                //throw new Exception("Invalid Token");
                return default(T);
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = KeyHelper.BuildRsaSigningKey(publicKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var data = jwtToken.Claims.First(x => x.Type == "data");
                var dataObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data.Value);
                return dataObj;
            }
            catch
            {
                throw new Exception("Invalid Token");
            }
        }
    }
}