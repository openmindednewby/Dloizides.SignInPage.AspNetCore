using System.Globalization;

namespace SignInPage.AspNetCore;

internal sealed record FieldSpec(string Id, string Field, string Type, string Autocomplete, string Label, SignInFieldRule? Rule, bool Autofocus = false);

internal static class FieldMarkup
{
    public static string Render(FieldSpec spec, SignInCopy copy, string? error)
    {
        var rule = spec.Rule ?? new SignInFieldRule();
        var rules = string.Concat(RuleAttributes(rule, copy, spec.Label).Select(attribute => " " + attribute));
        var focus = spec.Autofocus ? " autofocus" : string.Empty;
        var aria = error is null
            ? $" aria-describedby=\"{spec.Id}-error\""
            : $" aria-invalid=\"true\" aria-describedby=\"{spec.Id}-error\"";
        return $"""
            <label for="{spec.Id}">{Markup.Html(spec.Label)}</label>
            <input id="{spec.Id}" name="{spec.Id}" type="{spec.Type}" autocomplete="{spec.Autocomplete}" data-field="{spec.Field}"{rules}{focus}{aria} />
            <p id="{spec.Id}-error" class="field-error" aria-live="polite">{Markup.Html(error)}</p>
            """;
    }

    private static IEnumerable<string> RuleAttributes(SignInFieldRule rule, SignInCopy copy, string label)
    {
        if (rule.Required)
            yield return $"required data-required=\"true\" data-msg-required=\"{Markup.Html(FieldCheck.Format(copy.RequiredMessage, label, rule))}\"";
        if (FieldCheck.IsEmail(rule))
            yield return $"data-kind=\"email\" data-msg-email=\"{Markup.Html(FieldCheck.EmailMessage(copy, label, rule))}\"";
        if (rule.MinLength is { } min)
            yield return $"data-min=\"{min.ToString(CultureInfo.InvariantCulture)}\" data-msg-min=\"{Markup.Html(FieldCheck.Format(copy.TooShortMessage, label, rule))}\"";
        if (rule.MaxLength is { } max)
            yield return $"data-max=\"{max.ToString(CultureInfo.InvariantCulture)}\" data-msg-max=\"{Markup.Html(FieldCheck.Format(copy.TooLongMessage, label, rule))}\"";
        if (rule.Pattern is { } pattern)
            yield return $"data-pattern=\"{Markup.Html(pattern)}\" data-msg-pattern=\"{Markup.Html(FieldCheck.PatternMessage(copy, label, rule))}\"";
    }
}
