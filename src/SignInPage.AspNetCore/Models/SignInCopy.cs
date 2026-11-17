namespace SignInPage.AspNetCore;

/// <summary>Every user-visible text; messages take {field}, {min} and {max}, DemoRowAriaLabel takes {label}.</summary>
public sealed record SignInCopy
{
    public required string UsernameLabel { get; init; }

    public required string PasswordLabel { get; init; }

    public required string SubmitLabel { get; init; }

    public required string RequiredMessage { get; init; }

    public required string TooShortMessage { get; init; }

    public required string TooLongMessage { get; init; }

    public string? Footer { get; init; }

    public string? EmailInvalidMessage { get; init; }

    public string? RememberMeLabel { get; init; }

    public string? ForgotPasswordLink { get; init; }

    public string? ForgotPasswordTitle { get; init; }

    public string? ForgotPasswordSubmit { get; init; }

    public string? ForgotPasswordSent { get; init; }

    public string? ResetPasswordTitle { get; init; }

    public string? ResetPasswordSubmit { get; init; }

    public string? ResetPasswordDone { get; init; }

    public string? ResetPasswordInvalid { get; init; }

    public string? SignUpLink { get; init; }

    public string? DemoToggle { get; init; }

    public string? DemoHint { get; init; }

    public string? DemoAriaLabel { get; init; }

    public string? DemoRowAriaLabel { get; init; }
}
