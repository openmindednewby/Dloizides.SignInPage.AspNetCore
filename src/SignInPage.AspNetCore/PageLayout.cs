namespace SignInPage.AspNetCore;

internal static class PageLayout
{
    public static string Document(SignInPageShell shell, SignInTheme theme, string body) => $$"""
        <!DOCTYPE html>
        <html lang="en"><head><meta charset="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <meta name="robots" content="noindex, nofollow" />
        <title>{{Markup.Html(shell.Title)}}</title>
        <style>
        {{SignInStyles.Page(theme, shell.FontCssUrl)}}
        </style></head>
        <body>
        {{body}}
          <script type="module" src="{{SignInPageAssets.EntryScript}}"></script>
        </body></html>
        """;

    public static string Header(SignInPageShell shell, SignInMotion? motion)
    {
        var brand = shell.Brand;
        var accent = string.IsNullOrWhiteSpace(brand.Accent) ? string.Empty : $" <b>{Markup.Html(brand.Accent)}</b>";
        var tagline = string.IsNullOrWhiteSpace(brand.Tagline) ? string.Empty : $"<span>{Markup.Html(brand.Tagline)}</span>";
        return $"""
            {LogoMarkup.Slot(shell.Logo, motion)}
                <div class="brand">
                  <span class="brand-name">{Markup.Html(brand.Name)}{accent}</span>{tagline}
                </div>
            """;
    }

    public static string Error(string? error) =>
        error is null ? string.Empty : $"""<div class="err" role="alert">{Markup.Html(error)}</div>""";

    public static string Notice(string text) => $"""<p class="notice" role="status">{Markup.Html(text)}</p>""";

    public static string Link(string cssClass, string href, string? text) =>
        text is null ? string.Empty : $"""<a class="{cssClass}" href="{Markup.Html(CssValue.Link(nameof(href), href))}">{Markup.Html(text)}</a>""";
}
