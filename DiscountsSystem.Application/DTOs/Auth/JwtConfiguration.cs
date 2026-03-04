namespace DiscountsSystem.Application.DTOs.Auth;

public sealed class JwtConfiguration
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string Key { get; init; } = default!;
    public int ExpiresMinutes { get; init; }
}