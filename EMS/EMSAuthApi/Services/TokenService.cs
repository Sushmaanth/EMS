using Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EMSAuthApi.Services
{
    public class TokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),

                new Claim(ClaimTypes.Email, user.EmailId),

                new Claim(ClaimTypes.Role, user.Role.RoleName),

                new Claim("EmployeeId",user.EmployeeId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims:claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(configuration["Jwt:DurationInMinutes"])),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
