using FluentValidation;

namespace DiscountsSystem.Application.Validation.Common;

public static class PasswordRules
{
    public static IRuleBuilderOptions<T, string> ApplyStrongLatinPassword<T>(
        this IRuleBuilderInitial<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Must(NoSpaces).WithMessage("Password must not contain spaces.")
            .Must(BeLatinOnly).WithMessage("Password must contain only Latin letters, digits and ASCII symbols.")
            .Must(ContainUpper).WithMessage("Password must contain at least one uppercase letter.")
            .Must(ContainLower).WithMessage("Password must contain at least one lowercase letter.")
            .Must(ContainDigit).WithMessage("Password must contain at least one digit.")
            .Must(ContainSpecial).WithMessage("Password must contain at least one special character.");
    }

    private static bool NoSpaces(string? value)
        => !string.IsNullOrEmpty(value) && !value.Any(char.IsWhiteSpace);

    private static bool BeLatinOnly(string? value)
    {
        if (string.IsNullOrEmpty(value)) return false;

        foreach (var c in value)
        {
            if (char.IsWhiteSpace(c)) return false;

            if ((c >= 'A' && c <= 'Z') ||
                (c >= 'a' && c <= 'z') ||
                (c >= '0' && c <= '9') ||
                IsAsciiSpecial(c))
                continue;

            return false;
        }

        return true;
    }

    private static bool IsAsciiSpecial(char c)
        => (c >= '!' && c <= '/') ||
           (c >= ':' && c <= '@') ||
           (c >= '[' && c <= '`') ||
           (c >= '{' && c <= '~');

    private static bool ContainUpper(string? value)
        => !string.IsNullOrEmpty(value) && value.Any(c => c is >= 'A' and <= 'Z');

    private static bool ContainLower(string? value)
        => !string.IsNullOrEmpty(value) && value.Any(c => c is >= 'a' and <= 'z');

    private static bool ContainDigit(string? value)
        => !string.IsNullOrEmpty(value) && value.Any(c => c is >= '0' and <= '9');

    private static bool ContainSpecial(string? value)
        => !string.IsNullOrEmpty(value) && value.Any(IsAsciiSpecial);
}
