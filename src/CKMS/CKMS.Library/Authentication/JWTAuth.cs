using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CKMS.Library.Authentication
{
    public static class JWTAuth
    {
        public static String GenerateAdminJWTToken(String userId, String kitchenId, String roleId, String secretKey)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", userId),
                new Claim("roleId", roleId),
                new Claim("kitchenId", kitchenId),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64) // Issued At
            };

            return GenerateToken(claims, secretKey);
        }
        public static String GenerateCustomerJWTToken(String userId, String FullName, String secretKey)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", userId),
                new Claim("name", FullName),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64) // Issued At
            };

            return GenerateToken(claims, secretKey);
        }
        private static String GenerateToken(List<Claim> claims, String secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encrypterToken = tokenHandler.WriteToken(token);

            return encrypterToken;
        }
        public static String GenerateVerificationToken(String userId, String secretKey, String issuer, String audience, int expirationMinutes)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId), // Subject
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique ID
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64) // Issued At
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static (String UserId, String Token) ValidateToken(string token, String secretKey, String issuer, String audience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero // Optional: Adjust for token expiry buffer
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                String userId = principal.Claims.ToList()[0].Value;
                String tokenValue = principal?.FindFirstValue(JwtRegisteredClaimNames.Jti);
                return (userId, tokenValue);
            }
            catch (Exception ex)
            {
                return ("", ""); // Token validation failed
            }
        }
    }
}
