using DiscountsSystem.Domain.Enums;

namespace DiscountsSystem.Application.DTOs.AdminUsers;

public record CreateUserResponse(
    string Id,
    string Email,
    UserRole Role);