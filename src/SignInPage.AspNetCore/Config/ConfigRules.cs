using System.Globalization;
using System.Text.RegularExpressions;

namespace SignInPage.AspNetCore;

internal static class ConfigRules
{
    public static IEnumerable<SignInConfigError> CheckValidation(SignInValidation validation)
    {
        foreach (var (field, rule) in new[] { ("username", validation.Username), ("password", validation.Password) })
        {
            if (rule is null)
                continue;
            if (rule.Pattern is { } pattern && !Compiles(pattern))
                yield return new SignInConfigError($"/validation/{field}/pattern", "regex", $"{pattern} is not a valid regular expression");
            if (rule is { MinLength: { } min, MaxLength: { } max } && min > max)
                yield return new SignInConfigError($"/validation/{field}/minLength", "range", $"minLength {min} is above maxLength {max}");
        }
    }

    public static IEnumerable<SignInConfigError> CheckSessionCap(SignInFeatures features, SignInConfigParseOptions options)
    {
        if (features.RememberMe?.MaxHours is { } hours && options.SessionMaxHours is { } limit && hours > limit)
            yield return new SignInConfigError("/features/rememberMe/maxHours", "sessionCap",
                string.Create(CultureInfo.InvariantCulture, $"{hours} h is above the session limit of {limit} h"));
    }

    public static IEnumerable<SignInConfigError> CheckLinks(SignInFeatures features)
    {
        if (features.SignUp?.Href is { } href && !CssValue.IsSafeLink(href))
            yield return new SignInConfigError("/features/signUp/href", "url", $"{href} must be root-relative (one leading slash) or https");
    }

    private static bool Compiles(string pattern)
    {
        try
        {
            _ = new Regex(pattern, RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(250));
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
