namespace DiscountsSystem.Application.DTOs.Auth;


public record RegisterMerchantRequest(
    string CompanyName,
    string Email,
    string Password,
    string ConfirmPassword);