using System.Security.Claims;
using DiscountsSystem.Application.Interfaces.Common;
using DiscountsSystem.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DiscountsSystem.Infrastructure.Services.Common;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserService(IHttpContextAccessor http)
    {
        _http = http;
    }

    public bool IsAuthenticated =>
        _http.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public string? UserId =>
        _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? _http.HttpContext?.User?.FindFirstValue("sub"); // JWT standard fallback

    public UserRole? Role
    {
        get
        {
            var user = _http.HttpContext?.User;
            if (user is null) return null;

            var value =
                user.FindFirstValue(ClaimTypes.Role)
                ?? user.FindFirst("role")?.Value
                ?? user.FindFirst("roles")?.Value;

            if (string.IsNullOrWhiteSpace(value))
                return null;

            value = value.Trim();

            return Enum.TryParse<UserRole>(value, ignoreCase: true, out var role)
                ? role
                : null;
        }
    }
}
