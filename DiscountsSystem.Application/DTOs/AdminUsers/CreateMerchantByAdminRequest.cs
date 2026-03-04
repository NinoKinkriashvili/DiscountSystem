namespace DiscountsSystem.Application.DTOs.AdminUsers;

public record CreateMerchantByAdminRequest(
    string CompanyName,
    string Email,
    string Password,
    string ConfirmPassword);