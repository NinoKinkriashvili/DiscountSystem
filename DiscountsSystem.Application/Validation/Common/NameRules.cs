namespace DiscountsSystem.Application.Validation.Common;

public static class NameRules
{
    public static bool BeLatinNameWithSpaceOrHyphen(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        var s = value.Trim();
        if (s.Length == 0) return false;

        if (s[0] == '-' || s[^1] == '-') return false;

        bool prevWasSep = false;

        foreach (var c in s)
        {
            var isSep = c == ' ' || c == '-';

            if (isSep)
            {
                if (prevWasSep) return false;
                prevWasSep = true;
                continue;
            }

            // Latin
            if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                return false;

            prevWasSep = false;
        }

        return true;
    }
}
