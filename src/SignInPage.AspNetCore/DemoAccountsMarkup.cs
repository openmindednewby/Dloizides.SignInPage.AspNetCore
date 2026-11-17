using System.Globalization;

namespace SignInPage.AspNetCore;

internal static class DemoAccountsMarkup
{
    public static string Render(IReadOnlyList<SignInDemoAccount> accounts, SignInCopy copy)
    {
        if (accounts.Count == 0)
            return string.Empty;

        var rows = string.Concat(accounts.Select(account => Row(account, copy)));
        return $$"""
            <style>
              .demo-card { width:min(360px,100%); background:var(--card); border-radius:14px; padding:6px 10px;
                margin-top:14px; box-shadow:0 10px 40px var(--shadow); }
              .demo-card[open] { padding:6px 20px 16px; }
              .demo-toggle { cursor:pointer; list-style:none; min-height:44px; padding:12px 10px; font-size:.82rem;
                font-weight:600; color:var(--brand-on-light); display:flex; align-items:center; gap:8px; }
              .demo-toggle::-webkit-details-marker { display:none; }
              .demo-toggle::before { content:"\25B8"; font-size:.7rem; color:var(--muted);
                transition:transform .15s ease; }
              .demo-card[open] .demo-toggle::before { transform:rotate(90deg); }
              .demo-toggle:focus-visible { outline:none; border-radius:8px; box-shadow:0 0 0 3px var(--focus-ring); }
              .demo-hint { margin:0 0 4px; font-size:.72rem; color:var(--muted); }
              .demo-row { display:grid; grid-template-columns:1fr auto; gap:2px 10px; width:100%;
                text-align:left; align-items:center; background:var(--row-bg); border:1px solid var(--line);
                border-radius:9px; padding:9px 11px; margin-top:8px; cursor:pointer; font:inherit; }
              .demo-row:hover { border-color:var(--brand); background:var(--row-hover); }
              .demo-row:focus-visible { outline:none; border-color:var(--brand); box-shadow:0 0 0 3px var(--focus-ring); }
              .demo-row-label { grid-column:1; font-weight:600; font-size:.82rem; color:var(--ink); }
              .demo-row-user { grid-column:1; font-size:.74rem; color:var(--brand-on-light); }
              .demo-row-pass { grid-column:2; grid-row:1 / span 2; align-self:center; font-size:.74rem;
                color:var(--muted); background:var(--card); border:1px solid var(--line); border-radius:6px;
                padding:2px 6px; user-select:all; }
            </style>
            <details class="demo-card" aria-label="{{Markup.Html(copy.DemoAriaLabel)}}">
              <summary class="demo-toggle">{{Markup.Html(copy.DemoToggle)}}</summary>
              <p class="demo-hint">{{Markup.Html(copy.DemoHint)}}</p>
              {{rows}}
            </details>
            """;
    }

    private static string Row(SignInDemoAccount account, SignInCopy copy)
    {
        var shown = string.IsNullOrWhiteSpace(account.Label) ? account.Username : account.Label;
        var ariaLabel = Markup.Html(string.Format(CultureInfo.InvariantCulture, copy.DemoRowAriaLabel, shown));
        var label = Markup.Html(shown);
        var username = Markup.Html(account.Username);
        var password = Markup.Html(account.Password);
        return $"""
            <button type="button" class="demo-row" data-email="{username}" data-password="{password}" aria-label="{ariaLabel}">
              <span class="demo-row-label">{label}</span>
              <span class="demo-row-user">{username}</span>
              <code class="demo-row-pass">{password}</code>
            </button>
            """;
    }
}
