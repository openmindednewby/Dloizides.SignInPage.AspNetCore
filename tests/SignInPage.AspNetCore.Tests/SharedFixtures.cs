using System.Text.Json.Nodes;
using SignInPage.AspNetCore;

namespace SignInPage.AspNetCore.Tests;

internal static class SharedFixtures
{
    public static string Root() => Path.Combine(PackageFiles.RepoRoot(), "tests", "fixtures");

    public static TheoryData<string> Files(string group)
    {
        var data = new TheoryData<string>();
        foreach (var entry in Entries(group))
            data.Add(entry["file"]!.GetValue<string>());
        return data;
    }

    public static string Json(string file) => File.ReadAllText(Path.Combine(Root(), file));

    public static SignInConfigParseOptions Options(string group, string file)
    {
        var hours = Entry(group, file)["options"]?["sessionMaxHours"]?.GetValue<double>();
        return new SignInConfigParseOptions(hours);
    }

    public static IReadOnlyList<string> ExpectedErrors(string file) =>
        Entry("invalid", file)["errors"]!.AsArray()
            .Select(error => $"{error!["path"]!.GetValue<string>()}:{error["keyword"]!.GetValue<string>()}")
            .ToList();

    private static JsonObject Entry(string group, string file) =>
        Entries(group).Single(entry => entry["file"]!.GetValue<string>() == file);

    private static IEnumerable<JsonObject> Entries(string group) =>
        JsonNode.Parse(File.ReadAllText(Path.Combine(Root(), "manifest.json")))![group]!.AsArray().Select(node => node!.AsObject());
}
