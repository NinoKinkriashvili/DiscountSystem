using DiscountsSystem.Application.DTOs.Auth;
using DiscountsSystem.Application.Validation.Common;
using FluentValidation;

namespace DiscountsSystem.Application.Validation.Auth;

public sealed class RegisterMerchantRequestValidator : AbstractValidator<RegisterMerchantRequest>
{
    public RegisterMerchantRequestValidator()
    {
        RuleFor(x => x.CompanyName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Company name is required.")
            .MinimumLength(2).WithMessage("Company name must be at least 2 characters.")
            .MaximumLength(200).WithMessage("Company name is too long.");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.")
            .Must(EmailRules.BeAsciiEmail).WithMessage("Email must be a valid ASCII email (Latin letters/digits only).")
            .MaximumLength(256).WithMessage("Email is too long.");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .ApplyStrongLatinPassword();

        RuleFor(x => x.ConfirmPassword)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");
    }
}