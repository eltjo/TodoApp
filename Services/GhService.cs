using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using TodoApp.Models;

namespace TodoApp.Services;

public class GhService
{
    private static readonly string RepoOwner = "eltjo";
    private static readonly string RepoName = "TodoApp";

    public async Task<List<GitHubIssue>> GetIssuesAsync(string state = "open")
    {
        var json = await RunGhAsync(
            $"issue list --repo {RepoOwner}/{RepoName} --state {state} --limit 50 --json number,title,state,url,body"
        );

        if (string.IsNullOrWhiteSpace(json))
            return [];

        var raw = JsonSerializer.Deserialize<List<RawIssue>>(json, JsonOptions) ?? [];
        return raw.Select(r => new GitHubIssue
        {
            Number = r.Number,
            Title = r.Title,
            State = r.State,
            Url = r.Url,
            Body = r.Body
        }).ToList();
    }

    public async Task OpenIssueInBrowserAsync(string url)
    {
        await RunGhAsync($"browse {url}");
    }

    private static async Task<string> RunGhAsync(string arguments)
    {
        var tcs = new TaskCompletionSource<string>();

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "gh",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        process.Exited += async (_, _) =>
        {
            var output = await process.StandardOutput.ReadToEndAsync();
            tcs.TrySetResult(output);
            process.Dispose();
        };

        process.Start();
        return await tcs.Task;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private record RawIssue(
        [property: JsonPropertyName("number")] int Number,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("state")] string State,
        [property: JsonPropertyName("url")] string Url,
        [property: JsonPropertyName("body")] string Body
    );
}
