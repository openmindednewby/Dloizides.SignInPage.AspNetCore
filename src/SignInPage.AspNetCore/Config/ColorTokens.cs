namespace SignInPage.AspNetCore;

internal static class ColorTokens
{
    public static IReadOnlyList<(string Name, string CssName, string Value)> All(SignInColors c) =>
    [
        ("brand", "brand", c.Brand), ("brandHover", "brand-hover", c.BrandHover), ("brandInk", "brand-ink", c.BrandInk),
        ("brandOnLight", "brand-on-light", c.BrandOnLight), ("band", "band", c.Band), ("ink", "ink", c.Ink),
        ("label", "label", c.Label), ("line", "line", c.Line), ("muted", "muted", c.Muted), ("card", "card", c.Card),
        ("pageTop", "page-top", c.PageTop), ("pageBottom", "page-bottom", c.PageBottom), ("shadow", "shadow", c.Shadow),
        ("focusRing", "focus-ring", c.FocusRing), ("errorBackground", "error-bg", c.ErrorBackground),
        ("errorBorder", "error-border", c.ErrorBorder), ("errorText", "error-text", c.ErrorText),
        ("rowBackground", "row-bg", c.RowBackground), ("rowHover", "row-hover", c.RowHover), ("success", "success", c.Success),
    ];
}
