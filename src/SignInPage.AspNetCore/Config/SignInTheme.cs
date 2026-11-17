namespace SignInPage.AspNetCore;

/// <summary>Every colour the page paints, its font stack and optional corner radii in px.</summary>
public sealed record SignInTheme
{
    public required SignInColors Colors { get; init; }

    public required string FontFamily { get; init; }

    public SignInRadii? Radii { get; init; }
}

/// <summary>Corner radii in px for inputs and the card.</summary>
public sealed record SignInRadii
{
    public int? Input { get; init; }

    public int? Card { get; init; }
}
