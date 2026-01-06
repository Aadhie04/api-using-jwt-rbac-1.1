using APIUsingJWT.Data.Entities;
using APIUsingJWT.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace APIUsingJWT.Services.RepServices
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
        }
        public string GenerateAccessToken(string username, string role, int id, List<string> permissions)
        {
            _logger.LogInformation("Request to generate access token for {role}: {id}", role, id);
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim("UserId", id.ToString())
                };
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("permission", permission));
                }
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpiryMinute"]!)),
                    signingCredentials: creds
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Failed to generated access token for {role}: {id}", role, id);
                throw;
            }
        }
        public string GenerateRefreshToken()
        {
            _logger.LogInformation("Generating refresh token");
            //var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            //return refreshToken;
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
