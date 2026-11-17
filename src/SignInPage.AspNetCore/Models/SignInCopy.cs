namespace SignInPage.AspNetCore;

/// <summary>Every user-visible text on the page; DemoRowAriaLabel takes the account label as {0}.</summary>
public sealed record SignInCopy
{
    public required string UsernameLabel { get; init; }

    public required string PasswordLabel { get; init; }

    public required string SubmitLabel { get; init; }

    public string? Footer { get; init; }

    public required string DemoToggle { get; init; }

    public required string DemoHint { get; init; }

    public required string DemoAriaLabel { get; init; }

    public required string DemoRowAriaLabel { get; init; }
}
