namespace SignInPage.AspNetCore;

internal static class LogoMarkup
{
    public static string Slot(SignInLogo? logo, SignInMotion? motion)
    {
        if (logo is null)
            return string.Empty;

        var src = Markup.Html(logo.Src);
        var alt = Markup.Html(logo.Alt);
        var width = CssValue.Length(logo.Width);
        return $"""<div id="login-logo" class="login-logo" data-src="{src}" data-alt="{alt}" data-width="{width}"{MotionAttributes(motion)}><img src="{src}" alt="{alt}" style="width:{width};max-width:100%;height:auto" /></div>""";
    }

    private static string MotionAttributes(SignInMotion? motion) =>
        motion is null
            ? string.Empty
            : $" data-intro=\"{Markup.Html(motion.Intro)}\" data-micro=\"{Markup.Html(motion.Micro)}\" data-idle=\"{Markup.Html(motion.Idle)}\"";
}
