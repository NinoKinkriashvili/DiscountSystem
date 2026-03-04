using DiscountsSystem.Application.DTOs.Auth;
using DiscountsSystem.Application.Validation.Common;
using FluentValidation;

namespace DiscountsSystem.Application.Validation.Auth;

public sealed class RegisterCustomerRequestValidator : AbstractValidator<RegisterCustomerRequest>
{
    public RegisterCustomerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("First name is required.")
            .MinimumLength(1).WithMessage("First name must be at least 1 character.")
            .Must(NameRules.BeLatinNameWithSpaceOrHyphen)
            .WithMessage("First name must contain only letters and single spaces, hyphens (no digits or symbols).");

        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Last name is required.")
            .MinimumLength(4).WithMessage("Last name must be at least 4 characters.")
            .Must(NameRules.BeLatinNameWithSpaceOrHyphen)
            .WithMessage("Last name must contain only letters and single spaces, hyphens (no digits or symbols).");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.")
            .Must(EmailRules.BeAsciiEmail).WithMessage("Email must be a valid ASCII email (Latin letters and digits only).")
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