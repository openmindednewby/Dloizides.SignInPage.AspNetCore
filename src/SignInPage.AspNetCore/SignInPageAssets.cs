namespace SignInPage.AspNetCore;

/// <summary>Where the package serves its static web assets; the page loads <see cref="EntryScript"/>.</summary>
public static class SignInPageAssets
{
    public const string BasePath = "/_content/Dloizides.SignInPage.AspNetCore/login";

    public const string EntryScript = BasePath + "/sign-in.js";
}
