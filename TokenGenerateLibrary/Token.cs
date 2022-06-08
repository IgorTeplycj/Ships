﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace TokenGenerateLibrary
{
    public class Token
    {
        ClaimsIdentity GetIdentity(string id, string userName, string role)
        {
            var claims = new System.Collections.Generic.List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, id),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                };
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        public string GenerateToken(string id, string userName, string role)
        {
            var identity = GetIdentity(id, userName, role);

            //var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    //issuer: AuthOptions.ISSUER,
                    //audience: AuthOptions.AUDIENCE,
                    //notBefore: now,
                    claims: identity.Claims,
                    //expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                   );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }

        public bool ValidateToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            return true;
        }

        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidateActor = true,
                
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey() // The same key as the one that generate the token
            };
        }
    }
}
