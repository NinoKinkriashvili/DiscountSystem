using DiscountsSystem.Application.DTOs.AdminUsers;
using DiscountsSystem.Application.DTOs.Auth;

namespace DiscountsSystem.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterCustomerAsync(RegisterCustomerRequest request, CancellationToken ct = default);
    Task<AuthResponse> RegisterMerchantAsync(RegisterMerchantRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<CreateUserResponse> CreateCustomerByAdminAsync(CreateCustomerByAdminRequest request, CancellationToken ct = default);
    Task<CreateUserResponse> CreateMerchantByAdminAsync(CreateMerchantByAdminRequest request, CancellationToken ct = default);
}