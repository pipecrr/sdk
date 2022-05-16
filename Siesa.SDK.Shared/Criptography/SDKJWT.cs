using Microsoft.IdentityModel.Tokens;
using Siesa.SDK.Entities;
using Siesa.SDK.Shared.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Siesa.SDK.Shared.Criptography
{
    public class SDKJWT 
    {
        private readonly string _secretKey;
        private readonly long _minutesExp;

        public SDKJWT(string secretKey)
        {
            _secretKey = secretKey;
        }

        public SDKJWT(string secretKey, long minutesExp) : this(secretKey)
        {
            _minutesExp = minutesExp;
        }

        public JwtUserData? Validate(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userData = jwtToken.Claims.First(x => x.Type == "user");
                var user = Newtonsoft.Json.JsonConvert.DeserializeObject<JwtUserData>(userData.Value);
                return user;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }

        public string Generate(E00220_User? user, Dictionary<int, Dictionary<int, bool>>? featurePermissions, List<SessionRol> roles)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
             var userToken = new JwtUserData() {
                Rowid = user.Rowid,
                Path = user.Path,
                PasswordRecoveryEmail = user.PasswordRecoveryEmail,
                Name = user.Name,
                Id = user.Id,
                Description = user.Description,
                RowidCulture = user.RowidCulture,
                Roles = roles,
                FeaturePermissions = featurePermissions
                // Teams = user.Teams.Select(x => x.TeamName).ToArray(),
                //IsSuperAdmin = user.IsSuperAdmin
            };



            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { 
                    new Claim("user", Newtonsoft.Json.JsonConvert.SerializeObject(userToken)),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_minutesExp),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
