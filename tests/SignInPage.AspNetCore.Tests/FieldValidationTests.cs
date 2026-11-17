using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

public sealed class FieldValidationTests
{
    [Fact]
    public void Validate_WithEmptyRequiredUsername_ReturnsTheRequiredMessage()
    {
        var config = SignInPageSamples.Config() with
        {
            Copy = SignInPageSamples.Copy() with { UsernameLabel = "Email", RequiredMessage = "{field} is required" },
            Validation = new SignInValidation { Username = new SignInFieldRule { Required = true } },
        };

        var errors = SignInFieldValidator.Validate(config, "", "pw");

        Assert.Equal("Email is required", errors["username"]);
    }

    [Fact]
    public void Validate_WithEmailKindAndPlainText_ReturnsTheEmailMessage()
    {
        var config = SignInPageSamples.Config() with
        {
            Copy = SignInPageSamples.Copy() with { EmailInvalidMessage = "Enter a valid email address" },
            Validation = new SignInValidation { Username = new SignInFieldRule { Kind = "email" } },
        };

        var errors = SignInFieldValidator.Validate(config, "not-an-email", "pw");

        Assert.Equal("Enter a valid email address", errors["username"]);
    }

    [Fact]
    public void Validate_WithShortPassword_ReturnsTheTooShortMessage()
    {
        var config = SignInPageSamples.Config() with
        {
            Copy = SignInPageSamples.Copy() with { PasswordLabel = "Password", TooShortMessage = "{field} needs at least {min} characters" },
            Validation = new SignInValidation { Password = new SignInFieldRule { MinLength = 8 } },
        };

        var errors = SignInFieldValidator.Validate(config, "ana@acme.test", "short");

        Assert.Equal("Password needs at least 8 characters", errors["password"]);
    }

    [Fact]
    public void Validate_WithLongUsername_ReturnsTheTooLongMessage()
    {
        var config = SignInPageSamples.Config() with
        {
            Copy = SignInPageSamples.Copy() with { UsernameLabel = "Email", TooLongMessage = "{field} allows at most {max} characters" },
            Validation = new SignInValidation { Username = new SignInFieldRule { Kind = "text", MaxLength = 5 } },
        };

        var errors = SignInFieldValidator.Validate(config, "toolongname", "pw");

        Assert.Equal("Email allows at most 5 characters", errors["username"]);
    }

    [Fact]
    public void Validate_WithPatternMismatch_ReturnsThePatternMessage()
    {
        var config = SignInPageSamples.Config() with
        {
            Copy = SignInPageSamples.Copy() with { PasswordLabel = "Password" },
            Validation = new SignInValidation { Password = new SignInFieldRule { Pattern = "^(?=.*[0-9]).+$", PatternMessage = "{field} needs a digit" } },
        };

        var errors = SignInFieldValidator.Validate(config, "ana", "nodigits");

        Assert.Equal("Password needs a digit", errors["password"]);
    }

    [Fact]
    public void Validate_WithValidValues_ReturnsNoErrors()
    {
        var config = SignInPageSamples.Config() with
        {
            Validation = new SignInValidation { Username = new SignInFieldRule { Kind = "email" }, Password = new SignInFieldRule { MinLength = 8 } },
        };

        var errors = SignInFieldValidator.Validate(config, "ana@acme.test", "longenough");

        Assert.Empty(errors);
    }

    [Fact]
    public void Render_WithServerFieldError_ShowsItInlineAndAnnouncesIt()
    {
        var model = SignInPageSamples.Acme() with { FieldErrors = new Dictionary<string, string> { ["password"] = "Password needs a digit" } };

        var page = SignInPage.Render(model);

        Assert.Contains("aria-invalid=\"true\" aria-describedby=\"password-error\"", page);
        Assert.Contains("<p id=\"password-error\" class=\"field-error\" aria-live=\"polite\">Password needs a digit</p>", page);
    }

    [Fact]
    public void Render_WithMinLengthRule_EmitsTheClientCheckAttributes()
    {
        var config = SignInPageSamples.Config() with
        {
            Copy = SignInPageSamples.Copy() with { PasswordLabel = "Password", TooShortMessage = "{field} needs at least {min} characters" },
            Validation = new SignInValidation { Password = new SignInFieldRule { MinLength = 8 } },
        };

        var page = SignInPage.Render(SignInPageSamples.Acme() with { Config = config });

        Assert.Contains("data-min=\"8\" data-msg-min=\"Password needs at least 8 characters\"", page);
    }

    [Fact]
    public void FieldValidationScript_WhenShipped_IsWiredByTheEntryScript()
    {
        var module = File.ReadAllText(PackageFiles.LoginAsset("field-validation.js"));

        var entry = File.ReadAllText(PackageFiles.LoginAsset("sign-in.js"));

        Assert.Contains("import { wireFieldValidation } from './field-validation.js';", entry);
        Assert.Contains("export function fieldMessage(", module);
    }
}
