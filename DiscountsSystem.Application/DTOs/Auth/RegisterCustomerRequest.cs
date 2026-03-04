namespace DiscountsSystem.Application.DTOs.Auth;

public record RegisterCustomerRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword);