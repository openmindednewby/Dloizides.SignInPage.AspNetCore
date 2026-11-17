using System.Globalization;
using System.Text.RegularExpressions;

namespace SignInPage.AspNetCore;

internal static partial class FieldCheck
{
    public const string DefaultEmailInvalidMessage = "{field} is not a valid email address";

    private static readonly TimeSpan PatternTimeout = TimeSpan.FromMilliseconds(250);

    public static string? Message(SignInFieldRule? rule, SignInCopy copy, string label, string? value)
    {
        var checkedRule = rule ?? new SignInFieldRule();
        var text = value ?? string.Empty;
        if (text.Length == 0)
            return checkedRule.Required ? Format(copy.RequiredMessage, label, checkedRule) : null;
        if (IsEmail(checkedRule) && !EmailPattern().IsMatch(text))
            return EmailMessage(copy, label, checkedRule);
        var length = text.EnumerateRunes().Count();
        if (checkedRule.MinLength is { } min && length < min)
            return Format(copy.TooShortMessage, label, checkedRule);
        if (checkedRule.MaxLength is { } max && length > max)
            return Format(copy.TooLongMessage, label, checkedRule);
        if (checkedRule.Pattern is { } pattern && !Matches(pattern, text))
            return PatternMessage(copy, label, checkedRule);
        return null;
    }

    public static bool IsEmail(SignInFieldRule rule) => rule.Kind == "email";

    public static SignInFieldRule UsernameRule(SignInFieldRule? rule) =>
        rule is { Kind: not null } ? rule : (rule ?? new SignInFieldRule()) with { Kind = "email" };

    public static string EmailMessage(SignInCopy copy, string label, SignInFieldRule rule) =>
        Format(copy.EmailInvalidMessage ?? DefaultEmailInvalidMessage, label, rule);

    public static string PatternMessage(SignInCopy copy, string label, SignInFieldRule rule) =>
        Format(rule.PatternMessage ?? copy.RequiredMessage, label, rule);

    public static string Format(string template, string label, SignInFieldRule rule) =>
        template
            .Replace("{field}", label, StringComparison.Ordinal)
            .Replace("{min}", rule.MinLength?.ToString(CultureInfo.InvariantCulture) ?? string.Empty, StringComparison.Ordinal)
            .Replace("{max}", rule.MaxLength?.ToString(CultureInfo.InvariantCulture) ?? string.Empty, StringComparison.Ordinal);

    private static bool Matches(string pattern, string value)
    {
        try
        {
            return Regex.IsMatch(value, pattern, RegexOptions.CultureInvariant, PatternTimeout);
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    [GeneratedRegex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$")]
    private static partial Regex EmailPattern();
}
