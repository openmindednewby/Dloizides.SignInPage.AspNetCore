namespace SignInPage.AspNetCore;

/// <summary>The theme colour tokens; text tokens must reach 4.5:1 against the surface they sit on.</summary>
public sealed record SignInColors
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

    public required string Success { get; init; }
}
