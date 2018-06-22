using BillingSystem.Common;
using BillingSystem.Model.EntityDto;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BillingSystem.WebApi.Middleware
{
    public static class JwtMiddleware
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";

        public static TokenResponse GenerateToken(UserDto vm, int expireDays = 20)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;

            var identity = new ClaimsIdentity("JWT", vm.UserID.ToString(), "user");
            // add encrypted claims (Seen by server)
            identity.AddClaim(new Claim("userid", vm.UserID.ToString()));
            identity.AddClaim(new Claim("username", vm.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Email, vm.Email));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64));


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = now.AddDays(Convert.ToInt32(expireDays)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            var d = new TokenResponse
            {
                Email = vm.Email,
                AccessToken = token,
                ExpiredOn = tokenDescriptor.Expires,
                UserId = vm.UserID,
                FacilityId = vm.FacilityId,
                Name = vm.Name,
                FacilityName = vm.FacilityName,
                CorporateId = vm.CorporateId,
                CorporateName = vm.CorporateName,
            };
            return d;
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;
            }

            catch (Exception)
            {
                return null;
            }
        }
    }
}
