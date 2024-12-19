using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TAABP.Application.TokenGenerators;

namespace TAABP.Infrastructure
{
    public class JWTTokenGenerator
    {
        public class JwtTokenGenerator : ITokenGenerator
        {

            public IConfiguration _configuration;

            public JwtTokenGenerator(IConfiguration configuration)
            {
                this._configuration = configuration;
            }
            public string GenerateToken(string username, string password)
            {
                var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));

                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                var claims = new List<Claim>();
                claims.Add(new Claim("given name", username));

                var jwtSecurityToken = new JwtSecurityToken(
                    _configuration["Authentication:Issuer"],
                    _configuration["Authentication:Audience"],
                    claims,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddDays(1),
                    signingCredentials
                );
                var tokenHandler = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                return tokenHandler;
            }

            public bool ValidateToken(string token)
            {
                var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Authentication:Issuer"],
                    ValidAudience = _configuration["Authentication:Audience"],
                    IssuerSigningKey = securityKey
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                try
                {
                    tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Token validation failed: {ex.Message}");
                    return false;
                }

                return true;
            }
        }
    }
}
