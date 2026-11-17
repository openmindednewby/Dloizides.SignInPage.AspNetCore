using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class DemoAccountsTests
{
    [Fact]
    public void Render_WithoutDemoAccounts_HasNoDemoBlock()
    {
        var model = DemoModel(SignInPageSamples.Copy() with { DemoToggle = "Show test accounts" }) with { DemoAccounts = [] };

        var page = SignInPage.Render(model);

        Assert.DoesNotContain("demo-card", page);
        Assert.DoesNotContain("demo-row", page);
    }

    [Fact]
    public void Render_WithDemoAccount_RendersARowCarryingBothCredentials()
    {
        var copy = SignInPageSamples.Copy() with { DemoToggle = "Show test accounts", DemoRowAriaLabel = "Sign in as the {label} test account" };
        var model = DemoModel(copy) with { DemoAccounts = [new SignInDemoAccount("Analyst", "analyst@acme.test", "s3cret")] };

        var page = SignInPage.Render(model);

        Assert.Contains("<summary class=\"demo-toggle\">Show test accounts</summary>", page);
        Assert.Contains("data-email=\"analyst@acme.test\" data-password=\"s3cret\"", page);
        Assert.Contains("aria-label=\"Sign in as the Analyst test account\"", page);
    }

    [Fact]
    public void Render_WithDemoFeatureOff_HasNoDemoBlock()
    {
        var model = SignInPageSamples.Acme() with { DemoAccounts = [new SignInDemoAccount("Analyst", "analyst@acme.test", "s3cret")] };

        var page = SignInPage.Render(model);

        Assert.DoesNotContain("demo-card", page);
    }

    [Fact]
    public void Render_WithBlankDemoLabel_ShowsTheUsername()
    {
        var model = DemoModel(SignInPageSamples.Copy() with { DemoToggle = "Show test accounts" }) with { DemoAccounts = [new SignInDemoAccount(" ", "ops@acme.test", "pw")] };

        var page = SignInPage.Render(model);

        Assert.Contains("<span class=\"demo-row-label\">ops@acme.test</span>", page);
    }

    [Fact]
    public void Render_WithDemoAccounts_GivesTheToggleA44PixelMinimumHeight()
    {
        var model = DemoModel(SignInPageSamples.Copy() with { DemoToggle = "Show test accounts" }) with { DemoAccounts = [new SignInDemoAccount("Admin", "admin@acme.test", "pw")] };

        var page = SignInPage.Render(model);

        Assert.Matches(@"\.demo-toggle\s*\{[^}]*min-height:44px", page);
    }

    [Fact]
    public void DemoRowsScript_WhenShipped_FillsBothFieldsAndRequestsSubmit()
    {
        var path = PackageFiles.LoginAsset("demo-rows.js");

        var script = File.ReadAllText(path);

        Assert.Contains("export function wireDemoRows(", script);
        Assert.Contains("requestSubmit", script);
    }

    private static SignInPageModel DemoModel(SignInCopy copy) => SignInPageSamples.Acme() with
    {
        Config = SignInPageSamples.Config() with
        {
            Features = new SignInFeatures { DemoAccounts = new DemoAccountsFeature(true) },
            Copy = copy with { DemoHint = copy.DemoHint ?? "Click a row to sign in.", DemoAriaLabel = copy.DemoAriaLabel ?? "Test accounts", DemoRowAriaLabel = copy.DemoRowAriaLabel ?? "Sign in as {label}" },
        },
    };
}
