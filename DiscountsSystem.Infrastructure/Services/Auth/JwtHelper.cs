using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiscountsSystem.Application.DTOs.Auth;
using DiscountsSystem.Application.Interfaces.Auth;
using DiscountsSystem.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DiscountsSystem.Infrastructure.Services.Auth;

public sealed class JwtHelper : IJwtHelper
{
    private readonly JwtConfiguration _cfg;
    private readonly SymmetricSecurityKey _key;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtHelper(IOptions<JwtConfiguration> options)
    {
        _cfg = options.Value;

        if (string.IsNullOrWhiteSpace(_cfg.Key) || _cfg.Key.Length < 32)
            throw new InvalidOperationException("JWT Key must be at least 32 chars.");

        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg.Key));
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public (string Token, DateTime ExpiresAtUtc) GenerateToken(string userId, string email, UserRole role)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_cfg.ExpiresMinutes);
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.NameIdentifier, userId),

            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Email, email),

            new Claim(JwtRegisteredClaimNames.UniqueName, email),
            new Claim(ClaimTypes.Name, email),

            new Claim(ClaimTypes.Role, role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAtUtc,
            Issuer = _cfg.Issuer,
            Audience = _cfg.Audience,
            SigningCredentials = creds,
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return (_tokenHandler.WriteToken(token), expiresAtUtc);
    }
}
