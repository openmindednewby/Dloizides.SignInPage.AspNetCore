namespace SignInPage.AspNetCore;

/// <summary>Renders the complete sign-in HTML document from a <see cref="SignInPageModel"/>.</summary>
public static class SignInPage
{
    public static string Render(SignInPageModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var config = model.Config;
        var copy = config.Copy;
        var username = new FieldSpec("email", SignInFieldValidator.UsernameField, config.Validation.Username?.Kind == "text" ? "text" : "email",
            "username", copy.UsernameLabel, config.Validation.Username, Autofocus: true);
        var password = new FieldSpec("password", SignInFieldValidator.PasswordField, "password", "current-password", copy.PasswordLabel, config.Validation.Password);
        var body = $"""
              <form class="card" method="post" action="{Markup.Html(CssValue.Link(nameof(SignInForm.Action), model.Form.Action))}" novalidate>
                {PageLayout.Header(model.Shell, config.Motion)}
                {PageLayout.Error(model.Form.Error)}
                {FieldMarkup.Render(username, copy, model.FieldErrors.GetValueOrDefault(SignInFieldValidator.UsernameField))}
                {FieldMarkup.Render(password, copy, model.FieldErrors.GetValueOrDefault(SignInFieldValidator.PasswordField))}
                {RememberMarkup(config)}
                <input type="hidden" name="returnUrl" value="{Markup.Html(model.Form.ReturnUrl)}" />
                <button type="submit">{Markup.Html(copy.SubmitLabel)}</button>
                {LinksMarkup(model)}
                {FooterMarkup(copy.Footer)}
              </form>
              {DemoAccountsMarkup.Render(model.DemoAccounts, config)}
            """;
        return PageLayout.Document(model.Shell, config.Theme, body);
    }

    private static string RememberMarkup(SignInConfig config) =>
        config.Features.RememberMe is { Enabled: true }
            ? $"""<label class="remember" for="{RememberMe.FieldName}"><input id="{RememberMe.FieldName}" name="{RememberMe.FieldName}" type="checkbox" value="true" /> {Markup.Html(config.Copy.RememberMeLabel)}</label>"""
            : string.Empty;

    private static string LinksMarkup(SignInPageModel model)
    {
        var features = model.Config.Features;
        var forgot = features.ForgotPassword is { Enabled: true } ? PageLayout.Link("forgot", model.ForgotPasswordHref, model.Config.Copy.ForgotPasswordLink) : string.Empty;
        var signUp = features.SignUp is { Enabled: true, Href: { } href } ? PageLayout.Link("sign-up", href, model.Config.Copy.SignUpLink) : string.Empty;
        return forgot.Length + signUp.Length == 0 ? string.Empty : $"""<div class="links">{forgot}{signUp}</div>""";
    }

    private static string FooterMarkup(string? footer) =>
        string.IsNullOrWhiteSpace(footer) ? string.Empty : $"""<div class="foot">{Markup.Html(footer)}</div>""";
}
