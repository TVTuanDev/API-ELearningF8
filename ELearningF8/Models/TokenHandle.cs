using ELearningF8.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ELearningF8.Models
{
    public class TokenHandle
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _conf;

        public TokenHandle (AppDbContext context, IConfiguration conf)
        {
            _context = context;
            _conf = conf;
        }

        public string AccessToken(User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var roleNameDb = from r in _context.Roles
                             join ur in _context.UserRoles on r.Id equals ur.IdRole
                             where ur.IdUser == user.Id
                             select r.RoleName;

            var roleName = string.Join(",", roleNameDb);

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(ClaimTypes.Role, roleName)
            };

            var token = new JwtSecurityToken(
                issuer: _conf["Jwt:Issuer"],
                audience: _conf["Jwt:Audience"],
                claims: userClaims,
                expires: ExpriedToken.Access,
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string RefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        public string GetJti(string token)
        {
            var jwtToken = new JwtSecurityToken(token);
            var jti = jwtToken.Payload[JwtRegisteredClaimNames.Jti]?.ToString();
            if (!string.IsNullOrEmpty(jti)) return jti;
            return "";
        }
    }
}
