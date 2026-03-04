using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.Interfaces.Auth;

public interface IJwtHelper
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(string userId, string email, UserRole role);
}