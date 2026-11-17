using System.Security.Cryptography;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class LogoMotionTests
{
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
