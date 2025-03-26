using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BrainThrust.src.Models.Entities;
using Microsoft.IdentityModel.Tokens;

namespace BrainThrust.src.Services.Classes
{
    public static class JwtTokenService
    {
        private static readonly string SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET") 
            ?? throw new InvalidOperationException("JWT_SECRET is not set");

        private static readonly string Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "your-api";
        private static readonly string Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "your-client";

        public static string GenerateJwtToken(User user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(JwtRegisteredClaimNames.Iss, Issuer),  
                    new Claim(JwtRegisteredClaimNames.Aud, Audience)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = Issuer,  
                Audience = Audience,  
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
