using Microsoft.AspNetCore.WebUtilities;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

internal static class PasswordResetSamples
{
    public static PasswordResetPaths Paths(string origin) => new() { PublicOrigin = new Uri(origin) };

    public static SignInConfig Config(int tokenMinutes, int passwordMinLength) => SignInPageSamples.Config() with
    {
        Features = new SignInFeatures { ForgotPassword = new ForgotPasswordFeature(true, tokenMinutes) },
        Validation = new SignInValidation { Password = new SignInFieldRule { MinLength = passwordMinLength } },
        Copy = SignInPageSamples.Copy() with
        {
            ForgotPasswordLink = "Forgot password?",
            ForgotPasswordTitle = "Reset your password",
            ForgotPasswordSubmit = "Send reset link",
            ForgotPasswordSent = "A link is on its way.",
            ResetPasswordTitle = "Choose a new password",
            ResetPasswordSubmit = "Save password",
            ResetPasswordDone = "Your password is changed.",
            ResetPasswordInvalid = "This link has expired or was already used.",
        },
    };

    public static PasswordResetService Service(FakeResetHost host, SignInConfig config, TimeProvider time, PasswordResetQueue queue) =>
        new(host, config, time, queue, Paths("https://id.acme.test"));

    public static string TokenOf(Uri link) => QueryHelpers.ParseQuery(link.Query)["token"].ToString();
}
