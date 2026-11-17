using Microsoft.AspNetCore.Authentication;

namespace SignInPage.AspNetCore;

/// <summary>Cookie properties for remember-me: session cookie unless checked, capped at the session limit.</summary>
public static class RememberMe
{
    public const string FieldName = "rememberMe";

    public static AuthenticationProperties Properties(SignInConfig config, bool rememberMe, DateTimeOffset now, double? sessionMaxHours = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        if (!rememberMe || config.Features.RememberMe is not { Enabled: true, MaxHours: { } hours })
            return new AuthenticationProperties { IsPersistent = false };

        var capped = sessionMaxHours is { } limit ? Math.Min(hours, limit) : hours;
        return new AuthenticationProperties { IsPersistent = true, ExpiresUtc = now.AddHours(capped) };
    }
}
