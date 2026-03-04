using DiscountsSystem.Application.DTOs.AdminUsers;
using DiscountsSystem.Application.DTOs.Auth;
using DiscountsSystem.Application.Interfaces.Auth;
using DiscountsSystem.Domain.Enums;
using DiscountsSystem.Infrastructure.Persistence.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DiscountsSystem.Infrastructure.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtHelper _jwt;

    public AuthService(UserManager<AppUser> userManager, IJwtHelper jwt)
    {
        _userManager = userManager;
        _jwt = jwt;
    }

    public async Task<AuthResponse> RegisterCustomerAsync(RegisterCustomerRequest request, CancellationToken ct = default)
    {
        if (request.Password != request.ConfirmPassword)
            throw new InvalidOperationException("Passwords do not match.");

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            throw new InvalidOperationException("Email is already in use.");

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailConfirmed = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createRes = await _userManager.CreateAsync(user, request.Password);
        if (!createRes.Succeeded)
            throw new InvalidOperationException(ToErrorMessage(createRes));

        var roleRes = await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());
        if (!roleRes.Succeeded)
            throw new InvalidOperationException(ToErrorMessage(roleRes));

        return IssueToken(user, UserRole.Customer);
    }

    public async Task<AuthResponse> RegisterMerchantAsync(RegisterMerchantRequest request, CancellationToken ct = default)
    {
        if (request.Password != request.ConfirmPassword)
            throw new InvalidOperationException("Passwords do not match.");

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
            throw new InvalidOperationException("Email is already in use.");

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            CompanyName = request.CompanyName,
            EmailConfirmed = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createRes = await _userManager.CreateAsync(user, request.Password);
        if (!createRes.Succeeded)
            throw new InvalidOperationException(ToErrorMessage(createRes));

        var roleRes = await _userManager.AddToRoleAsync(user, UserRole.Merchant.ToString());
        if (!roleRes.Succeeded)
            throw new InvalidOperationException(ToErrorMessage(roleRes));

        return IssueToken(user, UserRole.Merchant);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            throw new InvalidOperationException("Invalid credentials.");

        var ok = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!ok)
            throw new InvalidOperationException("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(role))
            throw new InvalidOperationException("User has no role assigned.");

        if (!Enum.TryParse<UserRole>(role, out var parsedRole))
            throw new InvalidOperationException("Invalid user role.");

        return IssueToken(user, parsedRole);
    }


public async Task<CreateUserResponse> CreateCustomerByAdminAsync(CreateCustomerByAdminRequest request, CancellationToken ct = default)
{
    if (request.Password != request.ConfirmPassword)
        throw new InvalidOperationException("Passwords do not match.");

    var existing = await _userManager.FindByEmailAsync(request.Email);
    if (existing is not null)
        throw new InvalidOperationException("Email is already in use.");

    var user = new AppUser
    {
        UserName = request.Email,
        Email = request.Email,
        FirstName = request.FirstName,
        LastName = request.LastName,
        EmailConfirmed = true,
        CreatedAtUtc = DateTime.UtcNow
    };

    var createRes = await _userManager.CreateAsync(user, request.Password);
    if (!createRes.Succeeded)
        throw new InvalidOperationException(ToErrorMessage(createRes));

    var roleRes = await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());
    if (!roleRes.Succeeded)
        throw new InvalidOperationException(ToErrorMessage(roleRes));

    return new CreateUserResponse(user.Id, user.Email!, UserRole.Customer);
}

public async Task<CreateUserResponse> CreateMerchantByAdminAsync(CreateMerchantByAdminRequest request, CancellationToken ct = default)
{
    if (request.Password != request.ConfirmPassword)
        throw new InvalidOperationException("Passwords do not match.");

    var existing = await _userManager.FindByEmailAsync(request.Email);
    if (existing is not null)
        throw new InvalidOperationException("Email is already in use.");

    var user = new AppUser
    {
        UserName = request.Email,
        Email = request.Email,
        CompanyName = request.CompanyName,
        EmailConfirmed = true,
        CreatedAtUtc = DateTime.UtcNow
    };

    var createRes = await _userManager.CreateAsync(user, request.Password);
    if (!createRes.Succeeded)
        throw new InvalidOperationException(ToErrorMessage(createRes));

    var roleRes = await _userManager.AddToRoleAsync(user, UserRole.Merchant.ToString());
    if (!roleRes.Succeeded)
        throw new InvalidOperationException(ToErrorMessage(roleRes));

    return new CreateUserResponse(user.Id, user.Email!, UserRole.Merchant);
}

    private AuthResponse IssueToken(AppUser user, UserRole role)
    {
        var (token, expiresAtUtc) = _jwt.GenerateToken(user.Id, user.Email!, role);

        return new AuthResponse(
            Token: token,
            ExpiresAtUtc: expiresAtUtc,
            Role: role);
    }

    private static string ToErrorMessage(IdentityResult res)
        => string.Join("; ", res.Errors.Select(e => $"{e.Code}: {e.Description}"));
}
