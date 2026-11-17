using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class BrandNeutralityTests
{
    [Fact]
    public void Render_WithAcmeBrand_ContainsOnlyTheGivenBrandText()
    {
        var model = SignInPageSamples.Acme() with
        {
            DemoAccounts = [new SignInDemoAccount("Admin", "admin@acme.test", "pw")],
            Logo = new SignInLogo("/acme.webp", "Acme", "200px"),
            Motion = new SignInMotion("glow-sweep", "glint", "slow-glint"),
        };

        var page = SignInPage.Render(model);

        Assert.Contains("Acme", page);
        Assert.DoesNotContain("AML", page, StringComparison.Ordinal);
        Assert.DoesNotContain("#38bdf8", page, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("private console", page, StringComparison.OrdinalIgnoreCase);
    }
}
