using System.Security.Cryptography;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class LogoMotionTests
{
    [Fact]
    public void Render_WithLogoAndMotion_EmitsTheStillLogoAndThePresetNames()
    {
        var model = SignInPageSamples.Acme() with
        {
            Logo = new SignInLogo("/acme-logo.webp", "Acme", "200px"),
            Motion = new SignInMotion("glow-sweep", "glint", "slow-glint"),
        };

        var page = SignInPage.Render(model);

        Assert.Contains("<img src=\"/acme-logo.webp\" alt=\"Acme\"", page);
        Assert.Contains("data-intro=\"glow-sweep\" data-micro=\"glint\" data-idle=\"slow-glint\"", page);
    }

    [Fact]
    public void Render_WithLogoWithoutMotion_EmitsNoPresetNames()
    {
        var model = SignInPageSamples.Acme() with { Logo = new SignInLogo("/acme-logo.webp", "Acme", "200px"), Motion = null };

        var page = SignInPage.Render(model);

        Assert.Contains("id=\"login-logo\"", page);
        Assert.DoesNotContain("data-intro", page);
    }

    [Fact]
    public void Render_WithoutLogo_HasNoLogoSlot()
    {
        var model = SignInPageSamples.Acme() with { Logo = null };

        var page = SignInPage.Render(model);

        Assert.DoesNotContain("id=\"login-logo\"", page);
    }

    [Fact]
    public void Render_WithAnyModel_LoadsTheEntryScriptAndSubmitsOnEnter()
    {
        var model = SignInPageSamples.Acme() with { Form = new SignInForm("/login"), Copy = SignInPageSamples.Copy() with { SubmitLabel = "Sign in" } };

        var page = SignInPage.Render(model);

        Assert.Contains("<script type=\"module\" src=\"/_content/Dloizides.SignInPage.AspNetCore/login/sign-in.js\"></script>", page);
        Assert.Matches("<form class=\"card\" method=\"post\" action=\"/login\"[^>]*>[\\s\\S]*<button type=\"submit\">Sign in</button>[\\s\\S]*</form>", page);
    }

    [Fact]
    public void MotionModule_WhenShipped_ReadsPresetsFromTheSlotAndHonoursReducedMotion()
    {
        var path = PackageFiles.LoginAsset("login-motion.js");

        var module = File.ReadAllText(path);

        Assert.Contains("from './vendor/logo-kit-runtime.js'", module);
        Assert.Contains("from './login-submit.js'", module);
        Assert.Contains("slot.dataset", module);
        Assert.Contains("prefers-reduced-motion: reduce", module);
    }

    [Theory]
    [InlineData("startRasterMotion")]
    [InlineData("rasterMarkup")]
    [InlineData("rasterMotion")]
    public void LogoKitRuntime_WhenShipped_ExportsTheRasterApi(string export)
    {
        var path = PackageFiles.LoginAsset("vendor/logo-kit-runtime.js");

        var runtime = File.ReadAllText(path);

        Assert.Contains($"function {export}(", runtime);
    }

    [Fact]
    public void LogoKitRuntime_WhenVendored_IsByteForByteLogoKit060DistRuntime()
    {
        const string logoKit060RuntimeSha256 = "6448c669492cbabb589a3ad5c21951ca292c38f3bfc42a368b0d86716354f115";

        var actual = Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(PackageFiles.LoginAsset("vendor/logo-kit-runtime.js"))));

        Assert.Equal(logoKit060RuntimeSha256, actual);
    }
}
