using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class BrandNeutralityTests
{
    [Fact]
    public void Render_WithAcmeBrand_ContainsOnlyTheGivenBrandText()
    {
        var copy = SignInPageSamples.Copy() with { DemoToggle = "Demo", DemoHint = "Hint", DemoAriaLabel = "Demo", DemoRowAriaLabel = "Sign in as {label}" };
        var config = SignInPageSamples.Config() with
        {
            Copy = copy,
            Features = new SignInFeatures { DemoAccounts = new DemoAccountsFeature(true) },
            Motion = new SignInMotion { Enabled = true, Intro = new SignInMotionStep("stagger-rise", 600) },
        };
        var model = SignInPageSamples.Acme() with
        {
            Config = config,
            Shell = SignInPageSamples.Shell() with { Logo = new SignInLogo("/acme.webp", "Acme", "200px") },
            DemoAccounts = [new SignInDemoAccount("Admin", "admin@acme.test", "pw")],
        };

        var page = SignInPage.Render(model);

        Assert.Contains("Acme", page);
        Assert.DoesNotContain("AML", page, StringComparison.Ordinal);
        Assert.DoesNotContain("#38bdf8", page, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("private console", page, StringComparison.OrdinalIgnoreCase);
    }
}
