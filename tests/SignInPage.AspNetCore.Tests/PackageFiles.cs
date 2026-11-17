namespace SignInPage.AspNetCore.Tests;

internal static class PackageFiles
{
    public static string SourceRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "src", "SignInPage.AspNetCore")))
            dir = dir.Parent;
        Assert.NotNull(dir);
        return Path.Combine(dir.FullName, "src", "SignInPage.AspNetCore");
    }

    public static string LoginAsset(string name) => Path.Combine(SourceRoot(), "wwwroot", "login", name);
}
