using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;

namespace IzukaBus.Core.Crypto
{
    public class JWTHelper
    {
        internal static string GenerateJwtTokenWithClaims(string clientId, string username, string cryptoKey)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, username),
                new Claim(ClaimTypes.NameIdentifier, clientId),
                // Add more claims as needed
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cryptoKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "your-issuer",
                audience: "your-audience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
