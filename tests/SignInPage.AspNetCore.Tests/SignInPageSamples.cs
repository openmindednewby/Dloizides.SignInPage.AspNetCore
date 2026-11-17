using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

internal static class SignInPageSamples
{
    public static SignInTheme Theme() => new()
    {
        Brand = "#2563eb",
        BrandHover = "#1d4ed8",
        BrandInk = "#ffffff",
        BrandOnLight = "#1e40af",
        Band = "#ffffff",
        Ink = "#111827",
        Label = "#374151",
        Line = "#e5e7eb",
        Muted = "#6b7280",
        Card = "#ffffff",
        PageTop = "#111827",
        PageBottom = "#1f2937",
        Shadow = "rgba(0,0,0,.35)",
        FocusRing = "rgba(37,99,235,.15)",
        ErrorBackground = "#fef2f2",
        ErrorBorder = "#fecaca",
        ErrorText = "#991b1b",
        RowBackground = "#f9fafb",
        RowHover = "#f3f4f6",
        FontFamily = "\"Inter\", sans-serif",
    };

    public static SignInCopy Copy() => new()
    {
        UsernameLabel = "Email",
        PasswordLabel = "Password",
        SubmitLabel = "Sign in",
        Footer = "Acme staff only.",
        DemoToggle = "Show test accounts",
        DemoHint = "Click a row to sign in.",
        DemoAriaLabel = "Test accounts",
        DemoRowAriaLabel = "Sign in as the {0} test account",
    };

    public static SignInPageModel Acme() =>
        new("Sign in · Acme", new SignInBrand("Acme", "Cloud", "Identity"), Theme(), Copy(), new SignInForm("/login"));
}
