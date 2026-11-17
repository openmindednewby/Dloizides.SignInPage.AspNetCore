using System.Globalization;

namespace SignInPage.AspNetCore;

internal static class LogoMarkup
{
    public static string Slot(SignInLogo? logo, SignInMotion? motion)
    {
        if (logo is null)
            return string.Empty;

        var src = Markup.Html(CssValue.Link(nameof(SignInLogo.Src), logo.Src));
        var alt = Markup.Html(logo.Alt);
        var width = CssValue.Length(logo.Width);
        return $"""<div id="login-logo" class="login-logo" data-src="{src}" data-alt="{alt}" data-width="{width}"{MotionAttributes(motion)}><img src="{src}" alt="{alt}" style="width:{width};max-width:100%;height:auto" /></div>""";
    }

    private static string MotionAttributes(SignInMotion? motion)
    {
        if (motion is not { Enabled: true })
            return string.Empty;

        return Step("intro", motion.Intro?.Preset, motion.Intro?.DurationMs)
            + Step("micro", motion.Micro?.Preset, motion.Micro?.DurationMs)
            + Step("idle", motion.Idle?.Recipe, motion.Idle?.DurationMs);
    }

    private static string Step(string name, string? preset, int? durationMs)
    {
        if (preset is null)
            return string.Empty;
        var duration = durationMs is { } ms ? $" data-{name}-ms=\"{ms.ToString(CultureInfo.InvariantCulture)}\"" : string.Empty;
        return $" data-{name}=\"{Markup.Html(preset)}\"{duration}";
    }
}
