using Microsoft.IdentityModel.Tokens;
using Software_Solutions.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Software_Solutions.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateEmailVerficationToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        }

        public string CreateJWT(User user)
        {
            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                 new Claim(ClaimTypes.Role, user.Role),
                 new Claim(ClaimTypes.Email, user.Email)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(

                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds,
                claims: claims
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
          
        }


    }
}
