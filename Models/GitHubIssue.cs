namespace TodoApp.Models;

public class GitHubIssue
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    public string DisplayTitle => $"#{Number} {Title}";
    public string StateColor => State == "OPEN" ? "#2da44e" : "#8250df";
}
