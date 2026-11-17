namespace SignInPage.AspNetCore;

internal static class SignInStyles
{
    public static string Page(SignInTheme theme, string? fontCssUrl) => $$"""
        {{FontImport(fontCssUrl)}}
        :root { {{Variables(theme)}} }
        * { box-sizing:border-box; }
        body { margin:0; min-height:100vh; display:grid; place-items:center; padding:12px;
          font-family:var(--font);
          background:linear-gradient(180deg,var(--page-top),var(--page-bottom)); color:var(--ink); }
        .card { width:min(360px,100%); background:var(--card); border-radius:14px; padding:28px;
          box-shadow:0 10px 40px var(--shadow); }
        .brand { display:flex; align-items:center; gap:10px; margin-bottom:18px; }
        .brand b { font-size:1.05rem; } .brand span { color:var(--muted); font-size:.8rem; }
        label { display:block; font-size:.8rem; font-weight:600; color:var(--label); margin:12px 0 6px; }
        input { width:100%; padding:11px 12px; border:1px solid var(--line); border-radius:9px;
          font:inherit; outline:none; }
        input:focus { border-color:var(--brand); box-shadow:0 0 0 3px var(--focus-ring); }
        button { width:100%; margin-top:18px; padding:13px; min-height:44px; border:0; border-radius:9px;
          background:var(--brand); color:var(--brand-ink); font:inherit; font-weight:600; cursor:pointer; }
        button:hover { background:var(--brand-hover); }
        .login-logo { display:flex; justify-content:center; margin-bottom:14px; }
        .brand-name { font-weight:700; font-size:1.05rem; color:var(--ink); }
        .brand-name b { color:var(--brand-on-light); font-weight:700; }
        .err { background:var(--error-bg); border:1px solid var(--error-border); color:var(--error-text); padding:9px 12px;
          border-radius:9px; font-size:.85rem; margin-bottom:6px; }
        .foot { margin-top:14px; font-size:.72rem; color:var(--muted); text-align:center; }
        """;

    private static string FontImport(string? fontCssUrl) =>
        fontCssUrl is null ? string.Empty : $"""@import url("{CssValue.Url(fontCssUrl)}");""";

    private static string Variables(SignInTheme theme)
    {
        (string Name, string Value)[] colours =
        [
            ("brand", theme.Brand), ("brand-hover", theme.BrandHover), ("brand-ink", theme.BrandInk),
            ("brand-on-light", theme.BrandOnLight), ("band", theme.Band), ("ink", theme.Ink), ("label", theme.Label),
            ("line", theme.Line), ("muted", theme.Muted), ("card", theme.Card), ("page-top", theme.PageTop),
            ("page-bottom", theme.PageBottom), ("shadow", theme.Shadow), ("focus-ring", theme.FocusRing),
            ("error-bg", theme.ErrorBackground), ("error-border", theme.ErrorBorder), ("error-text", theme.ErrorText),
            ("row-bg", theme.RowBackground), ("row-hover", theme.RowHover),
        ];
        var declarations = colours.Select(colour => $"--{colour.Name}:{CssValue.Colour(colour.Name, colour.Value)};");
        return $"{string.Join(" ", declarations)} --font:{CssValue.FontFamily(theme.FontFamily)};";
    }
}
