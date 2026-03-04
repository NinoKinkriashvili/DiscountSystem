using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.Interfaces.Common;

public interface ICurrentUserService
{
    string? UserId { get; }
    UserRole? Role { get; }
    bool IsAuthenticated { get; }
}