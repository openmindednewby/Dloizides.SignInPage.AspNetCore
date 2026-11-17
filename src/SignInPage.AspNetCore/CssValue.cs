using System.Text.RegularExpressions;

namespace SignInPage.AspNetCore;

internal static partial class CssValue
{
    public static string Colour(string token, string value) => Checked(ColourPattern(), token, value, "a plain CSS colour");

    public static string FontFamily(string value) => Checked(FontFamilyPattern(), nameof(SignInTheme.FontFamily), value, "a font-family list");

    public static string Length(string value) => Checked(LengthPattern(), nameof(SignInLogo.Width), value, "a CSS length such as 200px");

    public static string Url(string value) => Checked(UrlPattern(), nameof(SignInPageModel.FontCssUrl), value, "an https or root-relative URL without quotes, brackets or spaces");

    private static string Checked(Regex pattern, string token, string value, string expected) =>
        value is not null && pattern.IsMatch(value)
            ? value
            : throw new ArgumentException($"{token} must be {expected}: '{value}'.", token);

    [GeneratedRegex(@"^(#[0-9a-fA-F]{3,8}|(rgb|rgba|hsl|hsla)\(\s*[0-9.,%\s/]+\)|[a-zA-Z]+)$")]
    private static partial Regex ColourPattern();

    [GeneratedRegex(@"^[A-Za-z0-9 ,""'\-]+$")]
    private static partial Regex FontFamilyPattern();

    [GeneratedRegex(@"^\d+(\.\d+)?(px|rem|em|%|vw)$")]
    private static partial Regex LengthPattern();

    [GeneratedRegex(@"^(https://|/)[^\s""'()<>\\]+$")]
    private static partial Regex UrlPattern();
}
