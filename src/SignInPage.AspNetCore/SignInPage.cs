namespace SignInPage.AspNetCore;

/// <summary>Renders the complete sign-in HTML document from a <see cref="SignInPageModel"/>.</summary>
public static class SignInPage
{
    public static string Render(SignInPageModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var copy = model.Copy;
        return $$"""
            <!DOCTYPE html>
            <html lang="en"><head><meta charset="utf-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1" />
            <meta name="robots" content="noindex, nofollow" />
            <title>{{Markup.Html(model.Title)}}</title>
            <style>
            {{SignInStyles.Page(model.Theme, model.FontCssUrl)}}
            </style></head>
            <body>
              <form class="card" method="post" action="{{Markup.Html(model.Form.Action)}}" autocomplete="off">
                {{LogoMarkup.Slot(model.Logo, model.Motion)}}
                <div class="brand">
                  {{BrandMarkup(model.Brand)}}
                </div>
                {{ErrorMarkup(model.Form.Error)}}
                <label for="email">{{Markup.Html(copy.UsernameLabel)}}</label>
                <input id="email" name="email" type="email" required autofocus />
                <label for="password">{{Markup.Html(copy.PasswordLabel)}}</label>
                <input id="password" name="password" type="password" required />
                <input type="hidden" name="returnUrl" value="{{Markup.Html(model.Form.ReturnUrl)}}" />
                <button type="submit">{{Markup.Html(copy.SubmitLabel)}}</button>
                {{FooterMarkup(copy.Footer)}}
              </form>
              {{DemoAccountsMarkup.Render(model.DemoAccounts, copy)}}
              <script type="module" src="{{SignInPageAssets.EntryScript}}"></script>
            </body></html>
            """;
    }

    private static string BrandMarkup(SignInBrand brand)
    {
        var accent = string.IsNullOrWhiteSpace(brand.Accent) ? string.Empty : $" <b>{Markup.Html(brand.Accent)}</b>";
        var tagline = string.IsNullOrWhiteSpace(brand.Tagline) ? string.Empty : $"<span>{Markup.Html(brand.Tagline)}</span>";
        return $"""<span class="brand-name">{Markup.Html(brand.Name)}{accent}</span>{tagline}""";
    }

    private static string ErrorMarkup(string? error) =>
        error is null ? string.Empty : $"""<div class="err">{Markup.Html(error)}</div>""";

    private static string FooterMarkup(string? footer) =>
        string.IsNullOrWhiteSpace(footer) ? string.Empty : $"""<div class="foot">{Markup.Html(footer)}</div>""";
}
