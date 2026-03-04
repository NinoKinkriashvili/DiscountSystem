using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.Auth;

public record AuthResponse(
    string Token,
    DateTime ExpiresAtUtc,
    UserRole Role);