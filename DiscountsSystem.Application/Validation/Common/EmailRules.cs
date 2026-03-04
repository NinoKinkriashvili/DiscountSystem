using System.Text.RegularExpressions;

namespace DiscountsSystem.Application.Validation.Common;

public static class EmailRules
{
    private static readonly Regex AsciiEmailRegex =
        new(@"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$", RegexOptions.Compiled);

    public static bool BeAsciiEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (value.Any(char.IsWhiteSpace)) return false;
        return AsciiEmailRegex.IsMatch(value);
    }
}