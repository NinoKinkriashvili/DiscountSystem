namespace DiscountsSystem.Application.DTOs.AdminUsers;

public record CreateCustomerByAdminRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword);