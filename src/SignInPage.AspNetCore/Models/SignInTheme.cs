namespace SignInPage.AspNetCore;

/// <summary>Every colour the page paints plus its font stack; each colour must be a plain CSS colour.</summary>
public sealed record SignInTheme
{
    public required string Brand { get; init; }

    public required string BrandHover { get; init; }

    public required string BrandInk { get; init; }

    public required string BrandOnLight { get; init; }

    public required string Band { get; init; }

    public required string Ink { get; init; }

    public required string Label { get; init; }

    public required string Line { get; init; }

    public required string Muted { get; init; }

    public required string Card { get; init; }

    public required string PageTop { get; init; }

    public required string PageBottom { get; init; }

    public required string Shadow { get; init; }

    public required string FocusRing { get; init; }

    public required string ErrorBackground { get; init; }

    public required string ErrorBorder { get; init; }

    public required string ErrorText { get; init; }

    public required string RowBackground { get; init; }

    public required string RowHover { get; init; }

    public required string FontFamily { get; init; }
}
