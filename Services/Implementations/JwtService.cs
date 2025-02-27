using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagementSystem.Services.Implementations
{
    public class JwtService
    {

        public static string GenerateToken(UserSession user, string role)
        {
            var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? throw new InvalidOperationException("SECRET_KEY environment variable is missing.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName, ClaimTypes.NameIdentifier),
                new Claim(JwtRegisteredClaimNames.Email, user.Email, ClaimTypes.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenSecurity = new JwtSecurityToken(issuer: "LibraryManagementSystem", audience: "LibraryManagementSystem", claims: claims, expires: DateTime.Now.AddMinutes(24 * 60), signingCredentials: creds);

            try
            {

                return new JwtSecurityTokenHandler().WriteToken(tokenSecurity);
            }
            catch (SecurityTokenEncryptionFailedException)
            {
                throw;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }

        }

        public static async Task ValidateToken(string token)
        {
            var secretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? throw new InvalidOperationException("SECRET_KEY environment variable is missing.");
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "LibraryManagementSystem",
                ValidAudience= "LibraryManagementSystem",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            try
            {
                await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
