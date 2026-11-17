namespace SignInPage.AspNetCore;

/// <summary>A validated sign-in.json: features, field rules, theme tokens, logo motion and copy.</summary>
public sealed record SignInConfig
{
    public required int Version { get; init; }

    public SignInFeatures Features { get; init; } = new();

    public SignInValidation Validation { get; init; } = new();

    public required SignInTheme Theme { get; init; }

    public SignInMotion? Motion { get; init; }

    public required SignInCopy Copy { get; init; }
}

/// <summary>The optional features of the page; a missing feature is off.</summary>
public sealed record SignInFeatures
{
    public RememberMeFeature? RememberMe { get; init; }

    public ForgotPasswordFeature? ForgotPassword { get; init; }

    public SignUpFeature? SignUp { get; init; }

    public DemoAccountsFeature? DemoAccounts { get; init; }
}

/// <summary>The remember-me checkbox; MaxHours caps the persistent cookie.</summary>
public sealed record RememberMeFeature(bool Enabled, double? MaxHours);

/// <summary>The forgot-password link and pages; TokenMinutes is the reset link lifetime (default 30).</summary>
public sealed record ForgotPasswordFeature(bool Enabled, int? TokenMinutes);

/// <summary>The sign-up link and where it points.</summary>
public sealed record SignUpFeature(bool Enabled, string? Href);

/// <summary>The demo-account list under the form.</summary>
public sealed record DemoAccountsFeature(bool Enabled);

/// <summary>The rules for the username and password fields.</summary>
public sealed record SignInValidation
{
    public SignInFieldRule? Username { get; init; }

    public SignInFieldRule? Password { get; init; }
}

/// <summary>One field's rules; Kind "email" makes the username an email address.</summary>
public sealed record SignInFieldRule
{
    public string? Kind { get; init; }

    public bool Required { get; init; } = true;

    public int? MinLength { get; init; }

    public int? MaxLength { get; init; }

    public string? Pattern { get; init; }

    public string? PatternMessage { get; init; }
}

/// <summary>Logo-kit presets for intro, submit glint and idle loop; Enabled false keeps the logo still.</summary>
public sealed record SignInMotion
{
    public bool Enabled { get; init; }

    public SignInMotionStep? Intro { get; init; }

    public SignInMotionStep? Micro { get; init; }

    public SignInIdleMotion? Idle { get; init; }
}

/// <summary>A motion preset and its optional duration in milliseconds.</summary>
public sealed record SignInMotionStep(string Preset, int? DurationMs);

/// <summary>An idle motion recipe and its optional duration in milliseconds.</summary>
public sealed record SignInIdleMotion(string Recipe, int? DurationMs);
