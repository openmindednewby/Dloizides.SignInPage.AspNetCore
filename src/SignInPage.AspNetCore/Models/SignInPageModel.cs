namespace SignInPage.AspNetCore;

/// <summary>Everything the sign-in page shows; the package adds layout, styles and scripts only.</summary>
public sealed record SignInPageModel(string Title, SignInBrand Brand, SignInTheme Theme, SignInCopy Copy, SignInForm Form)
{
    public SignInLogo? Logo { get; init; }

    public string? FontCssUrl { get; init; }

    public IReadOnlyList<SignInDemoAccount> DemoAccounts { get; init; } = [];

    public SignInMotion? Motion { get; init; }
}
