using System.Globalization;
using System.Text.RegularExpressions;

namespace SignInPage.AspNetCore;

internal static partial class ContrastCheck
{
    public const double MinTextContrast = 4.5;

    private static readonly (string Text, string Surface)[] Pairs =
    [
        ("ink", "card"), ("label", "card"), ("muted", "card"), ("brandOnLight", "card"), ("success", "card"),
        ("brandInk", "brand"), ("brandInk", "brandHover"), ("errorText", "errorBackground"),
        ("ink", "rowBackground"), ("ink", "rowHover"),
    ];

    private static readonly Rgba White = new(255, 255, 255, 1);

    public static IEnumerable<SignInConfigError> Check(SignInColors colors)
    {
        var errors = new List<SignInConfigError>();
        var parsed = new Dictionary<string, Rgba>();
        foreach (var (name, _, value) in ColorTokens.All(colors))
        {
            if (Parse(value) is { } rgba)
                parsed[name] = rgba;
            else
                errors.Add(new SignInConfigError(PathOf(name), "color", $"{value} is not a colour with channels 0-255"));
        }

        var card = Composite(parsed.GetValueOrDefault("card", Composite(parsed.GetValueOrDefault("pageTop", White), White)),
            Composite(parsed.GetValueOrDefault("pageTop", White), White));
        foreach (var (text, surface) in Pairs)
        {
            if (!parsed.TryGetValue(text, out var front))
                continue;
            Rgba? back = surface == "card" ? card : parsed.TryGetValue(surface, out var colour) ? Composite(colour, card) : null;
            if (back is not { } background)
                continue;
            var ratio = Ratio(front, background);
            if (ratio < MinTextContrast)
                errors.Add(new SignInConfigError(PathOf(text), "contrast",
                    string.Create(CultureInfo.InvariantCulture, $"contrast {ratio:F2}:1 against {surface}, needs {MinTextContrast}:1")));
        }

        return errors;
    }

    internal static Rgba? Parse(string value)
    {
        var text = value.Trim();
        if (HexPattern().IsMatch(text))
            return Hex(text[1..]);
        var match = FunctionalPattern().Match(text);
        if (!match.Success)
            return null;
        var (r, g, b) = (Channel(match, 1), Channel(match, 2), Channel(match, 3));
        if (r > 255 || g > 255 || b > 255)
            return null;
        var alpha = match.Groups[4].Success ? double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture) : 1;
        return new Rgba(r, g, b, alpha);
    }

    internal static double Ratio(Rgba foreground, Rgba background)
    {
        var baseColour = Composite(background, White);
        var top = Composite(foreground, baseColour);
        var (a, b) = (Luminance(top), Luminance(baseColour));
        return (Math.Max(a, b) + 0.05) / (Math.Min(a, b) + 0.05);
    }

    private static Rgba Hex(string hex)
    {
        var full = hex.Length <= 4 ? string.Concat(hex.Select(c => $"{c}{c}")) : hex;
        double Byte(int index) => int.Parse(full.AsSpan(index * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return new Rgba(Byte(0), Byte(1), Byte(2), full.Length == 8 ? Byte(3) / 255 : 1);
    }

    private static double Channel(Match match, int group) => double.Parse(match.Groups[group].Value, CultureInfo.InvariantCulture);

    private static Rgba Composite(Rgba top, Rgba bottom)
    {
        double Mix(double front, double back) => (front * top.A) + (back * (1 - top.A));
        return new Rgba(Mix(top.R, bottom.R), Mix(top.G, bottom.G), Mix(top.B, bottom.B), 1);
    }

    private static double Luminance(Rgba colour) => (0.2126 * Linear(colour.R)) + (0.7152 * Linear(colour.G)) + (0.0722 * Linear(colour.B));

    private static double Linear(double channel)
    {
        var s = channel / 255;
        return s <= 0.04045 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
    }

    private static string PathOf(string name) => $"/theme/colors/{name}";

    [GeneratedRegex("^#([0-9a-f]{3,4}|[0-9a-f]{6}|[0-9a-f]{8})$", RegexOptions.IgnoreCase)]
    private static partial Regex HexPattern();

    [GeneratedRegex(@"^rgba?\( *([0-9]{1,3}) *, *([0-9]{1,3}) *, *([0-9]{1,3}) *(?:, *(0|1|0?\.[0-9]+) *)?\)$")]
    private static partial Regex FunctionalPattern();

    internal readonly record struct Rgba(double R, double G, double B, double A);
}
