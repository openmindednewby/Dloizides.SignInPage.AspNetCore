namespace SignInPage.AspNetCore;

/// <summary>Everything the sign-in page shows; the package adds layout, styles and scripts only.</summary>
public sealed record SignInPageModel(SignInPageShell Shell, SignInConfig Config, SignInForm Form)
{
    public IReadOnlyList<SignInDemoAccount> DemoAccounts { get; init; } = [];

    public IReadOnlyDictionary<string, string> FieldErrors { get; init; } = new Dictionary<string, string>();

    public string ForgotPasswordHref { get; init; } = "/forgot-password";
}

/// <summary>Page title, optional brand (null or blank Name: logo only), optional logo and font CSS.</summary>
public sealed record SignInPageShell(string Title, SignInBrand? Brand)
{
    public SignInLogo? Logo { get; init; }

    public string? FontCssUrl { get; init; }
}
