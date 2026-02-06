using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WinDiet.Models;

namespace WinDiet.Services;

public class AuthService(IConfiguration configuration)
{
    private readonly string _secretKey = configuration["JwtSettings:SecretKey"]
                                         ?? throw new InvalidOperationException("JwtSettings:SecretKey is not configured");

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaimsIdentity(user),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(2),
            
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaimsIdentity(User user)
    {
        var ci = new ClaimsIdentity();
        ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        ci.AddClaim(new Claim(ClaimTypes.Name, user.Email));
        ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        ci.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));
        
        return ci;
    }
}