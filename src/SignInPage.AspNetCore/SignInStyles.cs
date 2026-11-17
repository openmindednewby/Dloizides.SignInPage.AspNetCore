using System.Globalization;

namespace SignInPage.AspNetCore;

internal static class SignInStyles
{
    private const int DefaultInputRadius = 9;

    private const int DefaultCardRadius = 14;

    public static string Page(SignInTheme theme, string? fontCssUrl) => $$"""
        {{FontImport(fontCssUrl)}}
        :root { {{Variables(theme)}} }
        * { box-sizing:border-box; }
        body { margin:0; min-height:100vh; display:grid; place-items:center; padding:12px;
          font-family:var(--font);
          background:linear-gradient(180deg,var(--page-top),var(--page-bottom)); color:var(--ink); }
        .card { width:min(360px,100%); background:var(--card); border-radius:var(--radius-card); padding:28px;
          box-shadow:0 10px 40px var(--shadow); }
        .brand { display:flex; align-items:center; gap:10px; margin-bottom:18px; }
        .brand b { font-size:1.05rem; } .brand span { color:var(--muted); font-size:.8rem; }
        .title { font-size:1.15rem; color:var(--ink); margin:0 0 6px; }
        label { display:block; font-size:.8rem; font-weight:600; color:var(--label); margin:12px 0 6px; }
        input { width:100%; padding:11px 12px; border:1px solid var(--line); border-radius:var(--radius-input);
          font:inherit; outline:none; }
        input:focus { border-color:var(--brand); box-shadow:0 0 0 3px var(--focus-ring); }
        input[aria-invalid="true"] { border-color:var(--error-border); }
        .field-error { margin:4px 0 0; font-size:.78rem; color:var(--error-text); }
        .field-error:empty { margin:0; }
        .remember { display:flex; align-items:center; gap:8px; min-height:44px; margin:8px 0 0; font-weight:400; color:var(--ink); }
        .remember input { width:20px; height:20px; padding:0; margin:0; }
        button { width:100%; margin-top:18px; padding:13px; min-height:44px; border:0; border-radius:var(--radius-input);
          background:var(--brand); color:var(--brand-ink); font:inherit; font-weight:600; cursor:pointer; }
        button:hover { background:var(--brand-hover); }
        .links { display:flex; justify-content:space-between; gap:12px; margin-top:8px; font-size:.8rem; }
        .links a { color:var(--brand-on-light); min-height:44px; min-width:44px; display:inline-flex; align-items:center; }
        .login-logo { display:flex; justify-content:center; margin-bottom:14px; }
        .brand-name { font-weight:700; font-size:1.05rem; color:var(--ink); }
        .brand-name b { color:var(--brand-on-light); font-weight:700; }
        .err { background:var(--error-bg); border:1px solid var(--error-border); color:var(--error-text); padding:9px 12px;
          border-radius:var(--radius-input); font-size:.85rem; margin-bottom:6px; }
        .notice { color:var(--success); font-size:.9rem; margin:6px 0; }
        .foot { margin-top:14px; font-size:.72rem; color:var(--muted); text-align:center; }
        """;

    private static string FontImport(string? fontCssUrl) =>
        fontCssUrl is null ? string.Empty : $"""@import url("{CssValue.Url(fontCssUrl)}");""";

    private static string Variables(SignInTheme theme)
    {
        var declarations = ColorTokens.All(theme.Colors).Select(colour => $"--{colour.CssName}:{CssValue.Colour(colour.Name, colour.Value)};");
        var inputRadius = (theme.Radii?.Input ?? DefaultInputRadius).ToString(CultureInfo.InvariantCulture);
        var cardRadius = (theme.Radii?.Card ?? DefaultCardRadius).ToString(CultureInfo.InvariantCulture);
        return $"{string.Join(" ", declarations)} --font:{CssValue.FontFamily(theme.FontFamily)}; --radius-input:{inputRadius}px; --radius-card:{cardRadius}px;";
    }
}
