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
using Newtonsoft.Json;

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
            if (String.IsNullOrEmpty(token)){
                return null;
            }
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

        public JwtSecurityToken ValidateToken(string token)
        {
            if (String.IsNullOrEmpty(token)){
                return null;
            }
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
                return jwtToken;
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Generates a JSON Web Token (JWT) for the specified user with the given permissions and roles.
        /// The token is valid for 7 days from the current UTC time.
        /// </summary>
        /// <param name="user">The user object representing the user's details.</param>
        /// <param name="featurePermissions">A dictionary containing feature names as keys and associated permission rowids as values.</param>
        /// <param name="roles">A list of SessionRol objects representing the roles assigned to the user.</param>
        /// <param name="rowidDbConnection">The short integer representing the database connection row ID.</param>
        /// <param name="rowidcompanygroup">Optional. The short integer representing the row ID of the company group. Default value is 0.</param>
        /// <param name="portalUser">Optional. An E00518_PortalUser object representing additional portal user details. Default value is null.</param>
        /// <param name="portalName">Optional. The name of the portal. Default value is an empty string.</param>
        /// <returns>A string representing the generated JSON Web Token.</returns>
        public string Generate(E00220_User? user, Dictionary<string, List<int>>? featurePermissions, List<SessionRol> roles,  short rowidDbConnection, short rowidcompanygroup =0, E00518_PortalUser portalUser = null, string portalName = "")
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            if(user == null){
                return "";
            }
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
                FeaturePermissions = featurePermissions,
                RowidCompanyGroup = rowidcompanygroup,
                RowIdDBConnection = rowidDbConnection,
                IsAdministrator = user.IsAdministrator,
                RowidAttachmentUserProfilePicture = user.RowidAttachmentUserProfilePicture
            };

            if(portalUser != null){
                PortalUserJwt portalUserJwt = new PortalUserJwt();
                portalUserJwt.Id = portalUser.ExternalUser.Id;
                portalUserJwt.RowidMainRecord = portalUser.RowidMainRecord;
                portalUserJwt.Email = portalUser.ExternalUser.Email;
                userToken.PortalUser = portalUserJwt;
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { 
                    new Claim("user", Newtonsoft.Json.JsonConvert.SerializeObject(userToken, Formatting.None))
                }),
                Expires = DateTime.UtcNow.AddMinutes(_minutesExp),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
