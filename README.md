# Dloizides.SignInPage.AspNetCore

Brand-neutral, server-rendered sign-in page for ASP.NET Core (net9.0, net10.0). MIT.

The package owns layout, CSS, the demo-account disclosure, the click-to-sign-in script and the
logo-kit raster motion with its submit glint. The host app passes everything brand-specific as data:
title, brand words, logo, theme colours, font, copy, form target and the demo accounts it chooses to publish.

```csharp
var html = SignInPage.Render(new SignInPageModel(
    "Sign in · Acme",
    new SignInBrand("Acme", Accent: "Cloud", Tagline: "Identity"),
    theme,
    copy,
    new SignInForm("/login", returnUrl, error))
{
    Logo = new SignInLogo("/login/acme-logo.webp", "Acme", "200px"),
    Motion = new SignInMotion("glow-sweep", "glint", "slow-glint"),
    FontCssUrl = "https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700&display=swap",
    DemoAccounts = accounts,
});
return Content(html, "text/html; charset=utf-8");
```

- Inputs post as `email`, `password` and `returnUrl`.
- Scripts and the vendored logo-kit runtime are static web assets under
  `/_content/Dloizides.SignInPage.AspNetCore/login/`; the host calls `app.UseStaticFiles()` (or `MapStaticAssets()`).
- Every colour comes from `SignInTheme`; a token that is not a plain CSS colour throws `ArgumentException`.
- Every text value is HTML-encoded.
- No demo accounts, no demo block. Clicking a demo row fills both fields and submits.
- Under `prefers-reduced-motion: reduce` the logo stays still and the form submits at once; otherwise the
  submit waits for the glint, at most 300 ms.

## 2.0: one sign-in.json

```csharp
builder.Services.AddSignInConfig("sign-in.json", new SignInConfigParseOptions(SessionMaxHours: 10));
builder.Services.AddSignInPasswordReset(new SignInPageShell("Sign in", new SignInBrand("Acme")), new PasswordResetPaths { PublicOrigin = new Uri("https://id.example.com") });
builder.Services.AddScoped<IPasswordResetHost, MyResetHost>();
app.MapSignInPasswordReset().RequireRateLimiting("password-reset");
```

The file follows the @dloizides/sign-in-config schema (embedded as `SignInConfigSchema.Json`). On login POST call `SignInFieldValidator.Validate` and pass errors back as `SignInPageModel.FieldErrors`; build cookie properties with `RememberMe.Properties`.

Reset links go only to the address `FindUserAsync` returns from your store and are built from `PublicOrigin`. Requests are queued and processed in the background, so the endpoint answers the same for every email. You must rate-limit the group (per IP and per email).
