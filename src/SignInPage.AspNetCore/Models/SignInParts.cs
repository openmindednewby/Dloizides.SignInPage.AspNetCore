namespace SignInPage.AspNetCore;

/// <summary>The product name, an optional emphasised second word and an optional tagline beside it.</summary>
public sealed record SignInBrand(string Name, string? Accent = null, string? Tagline = null);

/// <summary>Where the form posts, the return URL it carries and the error shown above the fields.</summary>
public sealed record SignInForm(string Action, string? ReturnUrl = null, string? Error = null);

/// <summary>The logo image shown above the form; Width is a CSS length such as 200px.</summary>
public sealed record SignInLogo(string Src, string Alt, string Width);

/// <summary>A test account listed under the form; clicking its row signs in as it. Blank Label shows Username.</summary>
public sealed record SignInDemoAccount(string Label, string Username, string Password);
