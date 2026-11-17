namespace SignInPage.AspNetCore;

internal static class PasswordResetPages
{
    public static string Request(SignInPageShell shell, SignInConfig config, PasswordResetPaths paths)
    {
        var copy = config.Copy;
        var rule = (config.Validation.Username ?? new SignInFieldRule()) with { Kind = "email", Pattern = null, PatternMessage = null };
        var email = new FieldSpec("email", SignInFieldValidator.UsernameField, "email", "email", copy.UsernameLabel, rule, Autofocus: true);
        return Card(shell, config, Text(copy.ForgotPasswordTitle), $"""
            <form class="card" method="post" action="{Markup.Html(CssValue.Link(nameof(paths.Forgot), paths.Forgot))}" novalidate>
            <!--header-->
            {FieldMarkup.Render(email, copy, null)}
            <button type="submit">{Markup.Html(copy.ForgotPasswordSubmit)}</button>
            <div class="links">{PageLayout.Link("sign-in", paths.SignIn, copy.SubmitLabel)}</div>
            </form>
            """);
    }

    public static string Sent(SignInPageShell shell, SignInConfig config, PasswordResetPaths paths) =>
        Card(shell, config, Text(config.Copy.ForgotPasswordTitle), $"""
            <div class="card">
            <!--header-->
            {PageLayout.Notice(Text(config.Copy.ForgotPasswordSent))}
            <div class="links">{PageLayout.Link("sign-in", paths.SignIn, config.Copy.SubmitLabel)}</div>
            </div>
            """);

    public static string Reset(SignInPageShell shell, SignInConfig config, PasswordResetPaths paths, string token, string? error)
    {
        var copy = config.Copy;
        var password = new FieldSpec("password", SignInFieldValidator.PasswordField, "password", "new-password", copy.PasswordLabel, config.Validation.Password, Autofocus: true);
        return Card(shell, config, Text(copy.ResetPasswordTitle), $"""
            <form class="card" method="post" action="{Markup.Html(CssValue.Link(nameof(paths.Reset), paths.Reset))}" novalidate>
            <!--header-->
            <input type="hidden" name="token" value="{Markup.Html(token)}" />
            {FieldMarkup.Render(password, copy, error)}
            <button type="submit">{Markup.Html(copy.ResetPasswordSubmit)}</button>
            </form>
            """);
    }

    public static string Done(SignInPageShell shell, SignInConfig config, PasswordResetPaths paths) =>
        Card(shell, config, Text(config.Copy.ResetPasswordTitle), $"""
            <div class="card">
            <!--header-->
            {PageLayout.Notice(Text(config.Copy.ResetPasswordDone))}
            <div class="links">{PageLayout.Link("sign-in", paths.SignIn, config.Copy.SubmitLabel)}</div>
            </div>
            """);

    public static string Invalid(SignInPageShell shell, SignInConfig config, PasswordResetPaths paths) =>
        Card(shell, config, Text(config.Copy.ResetPasswordTitle), $"""
            <div class="card">
            <!--header-->
            {PageLayout.Error(Text(config.Copy.ResetPasswordInvalid))}
            <div class="links">{PageLayout.Link("forgot", paths.Forgot, config.Copy.ForgotPasswordLink)}</div>
            </div>
            """);

    private static string Card(SignInPageShell shell, SignInConfig config, string title, string body)
    {
        var header = $"""{PageLayout.Header(shell, config.Motion)}<h1 class="title">{Markup.Html(title)}</h1>""";
        return PageLayout.Document(shell with { Title = $"{title} · {shell.Brand.Name}" }, config.Theme, body.Replace("<!--header-->", header, StringComparison.Ordinal));
    }

    private static string Text(string? value) =>
        value ?? throw new InvalidOperationException("Forgot-password copy is missing; enable features.forgotPassword in sign-in.json so its copy is required.");
}
